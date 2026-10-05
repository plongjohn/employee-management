using EmployeeManagement.Core.Models;
using EmployeeManagement.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Core.Services;

// Log messages contain ids only – names and email addresses are personal data and stay out of the logs.
public sealed class EmployeeService(
    IEmployeeRepository employeeRepository,
    EmployeeValidator validator,
    ILogger<EmployeeService> logger) : IEmployeeService
{
    public Task<PagedResult<Employee>> SearchAsync(EmployeeQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(query.Page, 1, nameof(query.Page));
        ArgumentOutOfRangeException.ThrowIfLessThan(query.PageSize, 1, nameof(query.PageSize));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(query.PageSize, EmployeeQuery.MaxPageSize, nameof(query.PageSize));

        return employeeRepository.SearchAsync(query, cancellationToken);
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        employeeRepository.GetByIdAsync(id, cancellationToken);

    public async Task<OperationResult<Employee>> CreateAsync(
        EmployeeInput input, CancellationToken cancellationToken = default)
    {
        var trimmed = input.Trimmed();
        var errors = validator.Validate(trimmed);
        if (errors.Count > 0)
        {
            return OperationResult<Employee>.ValidationFailed(errors);
        }

        try
        {
            var id = await employeeRepository.InsertAsync(trimmed, cancellationToken);
            logger.LogInformation("Created employee {EmployeeId}", id);
            return await ReloadAsync(id, cancellationToken);
        }
        catch (DuplicateEmailException)
        {
            logger.LogWarning("Rejected new employee: email address already in use");
            return OperationResult<Employee>.Failure(OperationStatus.DuplicateEmail);
        }
        catch (DepartmentNotFoundException)
        {
            logger.LogWarning("Rejected new employee: department no longer exists");
            return OperationResult<Employee>.ValidationFailed([ValidationError.DepartmentNotFound]);
        }
    }

    public async Task<OperationResult<Employee>> UpdateAsync(
        int id, uint version, EmployeeInput input, CancellationToken cancellationToken = default)
    {
        var trimmed = input.Trimmed();
        var errors = validator.Validate(trimmed);
        if (errors.Count > 0)
        {
            return OperationResult<Employee>.ValidationFailed(errors);
        }

        try
        {
            if (!await employeeRepository.UpdateAsync(id, version, trimmed, cancellationToken))
            {
                var status = await ExplainMissedWriteAsync(id, cancellationToken);
                logger.LogWarning("Update of employee {EmployeeId} failed: {Status}", id, status);
                return OperationResult<Employee>.Failure(status);
            }

            logger.LogInformation("Updated employee {EmployeeId}", id);
            return await ReloadAsync(id, cancellationToken);
        }
        catch (DuplicateEmailException)
        {
            logger.LogWarning("Update of employee {EmployeeId} rejected: email address already in use", id);
            return OperationResult<Employee>.Failure(OperationStatus.DuplicateEmail);
        }
        catch (DepartmentNotFoundException)
        {
            logger.LogWarning("Update of employee {EmployeeId} rejected: department no longer exists", id);
            return OperationResult<Employee>.ValidationFailed([ValidationError.DepartmentNotFound]);
        }
    }

    public async Task<OperationResult> DeleteAsync(int id, uint version, CancellationToken cancellationToken = default)
    {
        if (!await employeeRepository.DeleteAsync(id, version, cancellationToken))
        {
            var status = await ExplainMissedWriteAsync(id, cancellationToken);
            logger.LogWarning("Delete of employee {EmployeeId} failed: {Status}", id, status);
            return OperationResult.Failure(status);
        }

        logger.LogInformation("Deleted employee {EmployeeId}", id);
        return OperationResult.Success();
    }

    /// <summary>
    /// A write that matched no row either lost a race against a delete (NotFound) or against
    /// another update that already raised the version (Conflict).
    /// </summary>
    private async Task<OperationStatus> ExplainMissedWriteAsync(int id, CancellationToken cancellationToken)
    {
        var current = await employeeRepository.GetByIdAsync(id, cancellationToken);
        return current is null ? OperationStatus.NotFound : OperationStatus.Conflict;
    }

    // Reloading returns the values the database actually stored (new version, timestamps), so the
    // UI can keep editing without running into a false conflict on the next save.
    private async Task<OperationResult<Employee>> ReloadAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);
        return employee is null
            ? OperationResult<Employee>.Failure(OperationStatus.NotFound)
            : OperationResult<Employee>.Success(employee);
    }
}
