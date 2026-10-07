namespace EmployeeManagement.Core.Models;

public sealed class Employee
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required int DepartmentId { get; init; }
    public required string DepartmentName { get; init; }
    public required DateOnly HireDate { get; init; }
    public required uint Version { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }

    public EmployeeInput ToInput() => new(FirstName, LastName, Email, DepartmentId, HireDate);
}
