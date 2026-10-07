using EmployeeManagement.Core.Services;

namespace EmployeeManagement.Desktop.Resources;

internal static class ValidationMessages
{
    public static string For(ValidationError error) => error switch
    {
        ValidationError.FirstNameRequired => Strings.ValidationFirstNameRequired,
        ValidationError.FirstNameTooLong =>
            string.Format(Strings.ValidationFirstNameTooLong, EmployeeValidator.NameMaxLength),
        ValidationError.LastNameRequired => Strings.ValidationLastNameRequired,
        ValidationError.LastNameTooLong =>
            string.Format(Strings.ValidationLastNameTooLong, EmployeeValidator.NameMaxLength),
        ValidationError.EmailRequired => Strings.ValidationEmailRequired,
        ValidationError.EmailTooLong =>
            string.Format(Strings.ValidationEmailTooLong, EmployeeValidator.EmailMaxLength),
        ValidationError.EmailInvalid => Strings.ValidationEmailInvalid,
        ValidationError.DepartmentRequired => Strings.ValidationDepartmentRequired,
        ValidationError.DepartmentNotFound => Strings.ValidationDepartmentNotFound,
        ValidationError.HireDateTooEarly => string.Format(
            Strings.ValidationHireDateTooEarly, EmployeeValidator.EarliestHireDate.ToString(Strings.DateFormat)),
        ValidationError.HireDateTooFarInFuture => Strings.ValidationHireDateTooFarInFuture,
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, null),
    };
}
