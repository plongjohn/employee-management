using EmployeeManagement.Core.Data;

namespace EmployeeManagement.Core.Tests;

public class SqlLikeTests
{
    [Theory]
    [InlineData("mül", "mül%")]
    [InlineData("50%", @"50\%%")]
    [InlineData("first_last", @"first\_last%")]
    [InlineData(@"back\slash", @"back\\slash%")]
    [InlineData(@"\%", @"\\\%%")]
    public void Prefix_Term_EscapesWildcardsAndAppendsPercent(string term, string expected)
    {
        Assert.Equal(expected, SqlLike.Prefix(term));
    }
}
