using System.Text.RegularExpressions;
using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Services;

public sealed partial class EmployeeValidator(TimeProvider timeProvider)
{
    // Match the column sizes in database/setup.sql – change both together.
    public const int NameMaxLength = 100;
    public const int EmailMaxLength = 255;

    public static readonly DateOnly EarliestHireDate = new(1950, 1, 1);

    // Planned hires may be entered in advance, but not further ahead than a year.
    public const int MaxYearsInFuture = 1;

    public IReadOnlyList<ValidationError> Validate(EmployeeInput input)
    {
        var errors = new List<ValidationError>();

        ValidateName(input.FirstName, ValidationError.FirstNameRequired, ValidationError.FirstNameTooLong, errors);
        ValidateName(input.LastName, ValidationError.LastNameRequired, ValidationError.LastNameTooLong, errors);
        ValidateEmail(input.Email, errors);
        ValidateDepartment(input.DepartmentId, errors);
        ValidateHireDate(input.HireDate, errors);

        return errors;
    }

    private static void ValidateName(
        string name, ValidationError required, ValidationError tooLong, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(required);
        }
        else if (name.Length > NameMaxLength)
        {
            errors.Add(tooLong);
        }
    }

    private static void ValidateEmail(string email, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(ValidationError.EmailRequired);
        }
        else if (email.Length > EmailMaxLength)
        {
            errors.Add(ValidationError.EmailTooLong);
        }
        else if (!EmailPattern().IsMatch(email))
        {
            errors.Add(ValidationError.EmailInvalid);
        }
    }

    private static void ValidateDepartment(int departmentId, List<ValidationError> errors)
    {
        if (departmentId <= 0)
        {
            errors.Add(ValidationError.DepartmentRequired);
        }
    }

    private void ValidateHireDate(DateOnly hireDate, List<ValidationError> errors)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

        if (hireDate < EarliestHireDate)
        {
            errors.Add(ValidationError.HireDateTooEarly);
        }
        else if (hireDate > today.AddYears(MaxYearsInFuture))
        {
            errors.Add(ValidationError.HireDateTooFarInFuture);
        }
    }

    // Deliberately simple and identical to the web app's pattern, so both apps accept the
    // same addresses. Built-in validators (MailAddress, filter_var) disagree on edge cases.
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();
}
