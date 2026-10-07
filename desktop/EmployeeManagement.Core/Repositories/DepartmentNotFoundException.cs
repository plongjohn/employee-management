namespace EmployeeManagement.Core.Repositories;

public sealed class DepartmentNotFoundException(Exception innerException)
    : Exception("The selected department does not exist.", innerException);
