using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Services;

public interface IEmployeeService
{
    /// <exception cref="ArgumentOutOfRangeException">Page or page size outside the allowed range.</exception>
    Task<PagedResult<Employee>> SearchAsync(EmployeeQuery query, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <returns>On success, the employee as stored in the database.</returns>
    Task<OperationResult<Employee>> CreateAsync(EmployeeInput input, CancellationToken cancellationToken = default);

    /// <param name="version">The version the user loaded; a newer version in the database is a conflict.</param>
    /// <returns>On success, the employee as stored in the database, including its new version.</returns>
    Task<OperationResult<Employee>> UpdateAsync(
        int id, uint version, EmployeeInput input, CancellationToken cancellationToken = default);

    /// <param name="version">The version the user loaded; a newer version in the database is a conflict.</param>
    Task<OperationResult> DeleteAsync(int id, uint version, CancellationToken cancellationToken = default);
}
