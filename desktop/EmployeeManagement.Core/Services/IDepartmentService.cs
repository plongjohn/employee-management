using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Services;

public interface IDepartmentService
{
    /// <summary>All departments, sorted by name.</summary>
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default);
}
