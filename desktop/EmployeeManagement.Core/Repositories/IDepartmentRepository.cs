using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Repositories;

public interface IDepartmentRepository
{
    /// <summary>All departments, sorted by name.</summary>
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken);
}
