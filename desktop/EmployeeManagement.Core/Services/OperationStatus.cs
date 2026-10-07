namespace EmployeeManagement.Core.Services;

public enum OperationStatus
{
    Success,
    ValidationFailed,
    DuplicateEmail,

    /// <summary>The employee was changed by someone else since it was loaded.</summary>
    Conflict,

    /// <summary>The employee no longer exists, e.g. deleted by someone else.</summary>
    NotFound,
}
