using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Repositories;

/// <remarks>
/// Write methods throw <see cref="DuplicateEmailException"/> and
/// <see cref="DepartmentNotFoundException"/> instead of database-specific exceptions.
/// </remarks>
public interface IEmployeeRepository
{
    Task<PagedResult<Employee>> SearchAsync(EmployeeQuery query, CancellationToken cancellationToken);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <returns>The id of the new employee.</returns>
    Task<int> InsertAsync(EmployeeInput input, CancellationToken cancellationToken);

    /// <returns>False if no employee with this id and version exists.</returns>
    Task<bool> UpdateAsync(int id, uint expectedVersion, EmployeeInput input, CancellationToken cancellationToken);

    /// <returns>False if no employee with this id and version exists.</returns>
    Task<bool> DeleteAsync(int id, uint expectedVersion, CancellationToken cancellationToken);
}
