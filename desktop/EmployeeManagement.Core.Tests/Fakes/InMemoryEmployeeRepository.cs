using EmployeeManagement.Core.Models;
using EmployeeManagement.Core.Repositories;

namespace EmployeeManagement.Core.Tests.Fakes;

/// <summary>
/// Mimics the database rules the service relies on: version check, unique email
/// (case-insensitive like the collation) and the department foreign key.
/// </summary>
internal sealed class InMemoryEmployeeRepository : IEmployeeRepository
{
    private static readonly DateTime FixedTimestamp = new(2026, 10, 5, 12, 0, 0);

    private readonly Dictionary<int, string> _departments = new() { [1] = "Produktion", [2] = "IT" };
    private readonly Dictionary<int, Employee> _employees = [];
    private int _nextId = 1;

    public int WriteCount { get; private set; }
    public EmployeeQuery? LastQuery { get; private set; }

    public Employee Add(EmployeeInput input)
    {
        var employee = ToEmployee(_nextId++, input, version: 1);
        _employees[employee.Id] = employee;
        return employee;
    }

    /// <summary>Simulates another user saving the employee in the meantime.</summary>
    public void ChangeByOtherUser(int id) =>
        _employees[id] = ToEmployee(id, _employees[id].ToInput(), _employees[id].Version + 1);

    /// <summary>Simulates another user deleting the employee in the meantime.</summary>
    public void DeleteByOtherUser(int id) => _employees.Remove(id);

    public Task<PagedResult<Employee>> SearchAsync(EmployeeQuery query, CancellationToken cancellationToken)
    {
        LastQuery = query;
        var items = _employees.Values.ToList();
        return Task.FromResult(new PagedResult<Employee>(items, items.Count, query.Page, query.PageSize));
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        Task.FromResult(_employees.GetValueOrDefault(id));

    public Task<int> InsertAsync(EmployeeInput input, CancellationToken cancellationToken)
    {
        WriteCount++;
        EnsureDatabaseRules(input, ownId: null);
        return Task.FromResult(Add(input).Id);
    }

    public Task<bool> UpdateAsync(int id, uint expectedVersion, EmployeeInput input, CancellationToken cancellationToken)
    {
        WriteCount++;
        if (!_employees.TryGetValue(id, out var current) || current.Version != expectedVersion)
        {
            return Task.FromResult(false);
        }

        EnsureDatabaseRules(input, ownId: id);
        _employees[id] = ToEmployee(id, input, current.Version + 1);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, uint expectedVersion, CancellationToken cancellationToken)
    {
        WriteCount++;
        var matches = _employees.TryGetValue(id, out var current) && current.Version == expectedVersion;
        return Task.FromResult(matches && _employees.Remove(id));
    }

    private void EnsureDatabaseRules(EmployeeInput input, int? ownId)
    {
        var emailTaken = _employees.Values.Any(employee =>
            employee.Id != ownId && string.Equals(employee.Email, input.Email, StringComparison.OrdinalIgnoreCase));
        if (emailTaken)
        {
            throw new DuplicateEmailException(new InvalidOperationException("Duplicate entry"));
        }

        if (!_departments.ContainsKey(input.DepartmentId))
        {
            throw new DepartmentNotFoundException(new InvalidOperationException("Foreign key fails"));
        }
    }

    private Employee ToEmployee(int id, EmployeeInput input, uint version) => new()
    {
        Id = id,
        FirstName = input.FirstName,
        LastName = input.LastName,
        Email = input.Email,
        DepartmentId = input.DepartmentId,
        DepartmentName = _departments.GetValueOrDefault(input.DepartmentId, "?"),
        HireDate = input.HireDate,
        Version = version,
        CreatedAt = FixedTimestamp,
        UpdatedAt = FixedTimestamp,
    };
}
