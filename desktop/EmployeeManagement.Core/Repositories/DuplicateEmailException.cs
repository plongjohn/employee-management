namespace EmployeeManagement.Core.Repositories;

public sealed class DuplicateEmailException(Exception innerException)
    : Exception("Another employee already uses this email address.", innerException);
