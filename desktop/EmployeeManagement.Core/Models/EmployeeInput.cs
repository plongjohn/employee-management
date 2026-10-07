namespace EmployeeManagement.Core.Models;

/// <summary>
/// The values a user enters to create or change an employee. Being a record, two inputs
/// compare by value – the UI uses this to detect unsaved changes.
/// </summary>
public sealed record EmployeeInput(
    string FirstName,
    string LastName,
    string Email,
    int DepartmentId,
    DateOnly HireDate)
{
    public EmployeeInput Trimmed() => this with
    {
        FirstName = FirstName.Trim(),
        LastName = LastName.Trim(),
        Email = Email.Trim(),
    };
}
