using EmployeeManagement.Core.Models;
using EmployeeManagement.Core.Services;

namespace EmployeeManagement.Core.Tests;

public class EmployeeValidatorTests
{
    private static readonly DateOnly Today = new(2026, 10, 5);

    private readonly EmployeeValidator _validator = new(TestData.ClockAt(Today));

    [Fact]
    public void Validate_ValidInput_ReturnsNoErrors()
    {
        var errors = _validator.Validate(TestData.ValidInput());

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_BlankFirstName_ReturnsFirstNameRequired(string firstName)
    {
        var errors = _validator.Validate(TestData.ValidInput() with { FirstName = firstName });

        Assert.Equal([ValidationError.FirstNameRequired], errors);
    }

    [Fact]
    public void Validate_FirstNameAtMaxLength_ReturnsNoErrors()
    {
        var input = TestData.ValidInput() with { FirstName = new string('a', EmployeeValidator.NameMaxLength) };

        Assert.Empty(_validator.Validate(input));
    }

    [Fact]
    public void Validate_FirstNameOverMaxLength_ReturnsFirstNameTooLong()
    {
        var input = TestData.ValidInput() with { FirstName = new string('a', EmployeeValidator.NameMaxLength + 1) };

        Assert.Equal([ValidationError.FirstNameTooLong], _validator.Validate(input));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_BlankLastName_ReturnsLastNameRequired(string lastName)
    {
        var errors = _validator.Validate(TestData.ValidInput() with { LastName = lastName });

        Assert.Equal([ValidationError.LastNameRequired], errors);
    }

    [Fact]
    public void Validate_LastNameOverMaxLength_ReturnsLastNameTooLong()
    {
        var input = TestData.ValidInput() with { LastName = new string('a', EmployeeValidator.NameMaxLength + 1) };

        Assert.Equal([ValidationError.LastNameTooLong], _validator.Validate(input));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_BlankEmail_ReturnsEmailRequired(string email)
    {
        var errors = _validator.Validate(TestData.ValidInput() with { Email = email });

        Assert.Equal([ValidationError.EmailRequired], errors);
    }

    [Theory]
    [InlineData("anna.mueller@example.com")]
    [InlineData("o'connor@example.com")]
    [InlineData("a@b.co")]
    [InlineData("first.last+tag@sub.example.at")]
    public void Validate_WellFormedEmail_ReturnsNoErrors(string email)
    {
        var errors = _validator.Validate(TestData.ValidInput() with { Email = email });

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("anna.mueller")]
    [InlineData("anna.mueller@")]
    [InlineData("@example.com")]
    [InlineData("anna@example")]
    [InlineData("anna mueller@example.com")]
    [InlineData("anna@@example.com")]
    public void Validate_MalformedEmail_ReturnsEmailInvalid(string email)
    {
        var errors = _validator.Validate(TestData.ValidInput() with { Email = email });

        Assert.Equal([ValidationError.EmailInvalid], errors);
    }

    [Fact]
    public void Validate_EmailAtMaxLength_ReturnsNoErrors()
    {
        var input = TestData.ValidInput() with { Email = EmailOfLength(EmployeeValidator.EmailMaxLength) };

        Assert.Empty(_validator.Validate(input));
    }

    [Fact]
    public void Validate_EmailOverMaxLength_ReturnsEmailTooLong()
    {
        var input = TestData.ValidInput() with { Email = EmailOfLength(EmployeeValidator.EmailMaxLength + 1) };

        Assert.Equal([ValidationError.EmailTooLong], _validator.Validate(input));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_NoDepartment_ReturnsDepartmentRequired(int departmentId)
    {
        var errors = _validator.Validate(TestData.ValidInput() with { DepartmentId = departmentId });

        Assert.Equal([ValidationError.DepartmentRequired], errors);
    }

    [Fact]
    public void Validate_HireDateOnEarliestDate_ReturnsNoErrors()
    {
        var input = TestData.ValidInput() with { HireDate = EmployeeValidator.EarliestHireDate };

        Assert.Empty(_validator.Validate(input));
    }

    [Fact]
    public void Validate_HireDateBeforeEarliestDate_ReturnsHireDateTooEarly()
    {
        var input = TestData.ValidInput() with { HireDate = EmployeeValidator.EarliestHireDate.AddDays(-1) };

        Assert.Equal([ValidationError.HireDateTooEarly], _validator.Validate(input));
    }

    [Fact]
    public void Validate_HireDateOneYearAhead_ReturnsNoErrors()
    {
        var input = TestData.ValidInput() with { HireDate = Today.AddYears(1) };

        Assert.Empty(_validator.Validate(input));
    }

    [Fact]
    public void Validate_HireDateMoreThanOneYearAhead_ReturnsHireDateTooFarInFuture()
    {
        var input = TestData.ValidInput() with { HireDate = Today.AddYears(1).AddDays(1) };

        Assert.Equal([ValidationError.HireDateTooFarInFuture], _validator.Validate(input));
    }

    [Fact]
    public void Validate_SeveralInvalidFields_ReturnsAllErrors()
    {
        var input = new EmployeeInput("", "", "invalid", 0, EmployeeValidator.EarliestHireDate.AddDays(-1));

        var errors = _validator.Validate(input);

        Assert.Equal(
            [
                ValidationError.FirstNameRequired,
                ValidationError.LastNameRequired,
                ValidationError.EmailInvalid,
                ValidationError.DepartmentRequired,
                ValidationError.HireDateTooEarly,
            ],
            errors);
    }

    private static string EmailOfLength(int length)
    {
        const string domain = "@example.com";
        return new string('a', length - domain.Length) + domain;
    }
}
