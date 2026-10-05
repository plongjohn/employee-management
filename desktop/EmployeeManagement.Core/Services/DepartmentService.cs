using EmployeeManagement.Core.Models;
using EmployeeManagement.Core.Repositories;

namespace EmployeeManagement.Core.Services;

public sealed class DepartmentService(IDepartmentRepository departmentRepository) : IDepartmentService
{
    public Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default) =>
        departmentRepository.GetAllAsync(cancellationToken);
}
