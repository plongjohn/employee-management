using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Tests;

public class PagedResultTests
{
    [Theory]
    [InlineData(0, 25, 0)]
    [InlineData(1, 25, 1)]
    [InlineData(25, 25, 1)]
    [InlineData(26, 25, 2)]
    [InlineData(100, 50, 2)]
    public void TotalPages_TotalCount_RoundsUpToFullPages(int totalCount, int pageSize, int expectedPages)
    {
        var result = new PagedResult<int>([], totalCount, Page: 1, pageSize);

        Assert.Equal(expectedPages, result.TotalPages);
    }
}
