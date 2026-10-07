using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Tests;

public class EmployeePageExtensionsTests
{
    [Fact]
    public void HasSameContentAs_SameIdsAndVersions_ReturnsTrue()
    {
        var shown = Page(Employee(1, version: 1), Employee(2, version: 3));
        var reloaded = Page(Employee(1, version: 1), Employee(2, version: 3));

        Assert.True(shown.HasSameContentAs(reloaded));
    }

    [Fact]
    public void HasSameContentAs_EmployeeUpdated_ReturnsFalse()
    {
        var shown = Page(Employee(1, version: 1));
        var reloaded = Page(Employee(1, version: 2));

        Assert.False(shown.HasSameContentAs(reloaded));
    }

    [Fact]
    public void HasSameContentAs_EmployeeReplacedByAnother_ReturnsFalse()
    {
        var shown = Page(Employee(1, version: 1));
        var reloaded = Page(Employee(2, version: 1));

        Assert.False(shown.HasSameContentAs(reloaded));
    }

    [Fact]
    public void HasSameContentAs_OrderChanged_ReturnsFalse()
    {
        var shown = Page(Employee(1, version: 1), Employee(2, version: 1));
        var reloaded = Page(Employee(2, version: 1), Employee(1, version: 1));

        Assert.False(shown.HasSameContentAs(reloaded));
    }

    [Fact]
    public void HasSameContentAs_EmployeeAddedOnAnotherPage_ReturnsFalse()
    {
        var shown = Page(Employee(1, version: 1));
        var reloaded = shown with { TotalCount = shown.TotalCount + 1 };

        Assert.False(shown.HasSameContentAs(reloaded));
    }

    private static PagedResult<Employee> Page(params Employee[] employees) =>
        new(employees, TotalCount: employees.Length, Page: 1, PageSize: EmployeeQuery.DefaultPageSize);

    private static Employee Employee(int id, uint version) => new()
    {
        Id = id,
        FirstName = "Anna",
        LastName = "Müller",
        Email = $"employee{id}@example.com",
        DepartmentId = 1,
        DepartmentName = "Produktion",
        HireDate = new DateOnly(2020, 1, 15),
        Version = version,
        CreatedAt = new DateTime(2020, 1, 15),
        UpdatedAt = new DateTime(2020, 1, 15),
    };
}
