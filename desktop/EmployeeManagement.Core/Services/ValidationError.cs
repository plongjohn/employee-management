namespace EmployeeManagement.Core.Services;

/// <summary>
/// Language-neutral validation results. The UI maps each code to a field and a localized text.
/// </summary>
public enum ValidationError
{
    FirstNameRequired,
    FirstNameTooLong,
    LastNameRequired,
    LastNameTooLong,
    EmailRequired,
    EmailTooLong,
    EmailInvalid,
    DepartmentRequired,
    DepartmentNotFound,
    HireDateTooEarly,
    HireDateTooFarInFuture,
}
