using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Tests;

public class EmployeeInputTests
{
    [Fact]
    public void Trimmed_SurroundingWhitespace_IsRemovedFromTextFields()
    {
        var input = new EmployeeInput("  Anna ", "\tMüller ", " anna.mueller@example.com  ", 1, new DateOnly(2020, 1, 15));

        var trimmed = input.Trimmed();

        Assert.Equal(TestData.ValidInput(), trimmed);
    }

    [Fact]
    public void Equals_SameValues_IsTrue()
    {
        Assert.Equal(TestData.ValidInput(), TestData.ValidInput());
    }

    [Fact]
    public void Equals_OneFieldChanged_IsFalse()
    {
        Assert.NotEqual(TestData.ValidInput(), TestData.ValidInput() with { DepartmentId = 2 });
    }
}
