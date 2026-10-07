using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Tests;

public class EmployeeQueryTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SearchTerms_NoText_IsEmpty(string? searchText)
    {
        var query = new EmployeeQuery(SearchText: searchText);

        Assert.Empty(query.SearchTerms);
    }

    [Fact]
    public void SearchTerms_TextWithExtraWhitespace_SplitsIntoWords()
    {
        var query = new EmployeeQuery(SearchText: "  anna \t mü  ");

        Assert.Equal(["anna", "mü"], query.SearchTerms);
    }

    [Fact]
    public void SearchTerms_MoreWordsThanAllowed_KeepsFirstWords()
    {
        var query = new EmployeeQuery(SearchText: "a b c d e f g");

        Assert.Equal(["a", "b", "c", "d", "e"], query.SearchTerms);
    }

    [Theory]
    [InlineData(1, 25, 0)]
    [InlineData(2, 25, 25)]
    [InlineData(3, 50, 100)]
    public void Offset_PageAndPageSize_SkipsPreviousPages(int page, int pageSize, int expectedOffset)
    {
        var query = new EmployeeQuery(Page: page, PageSize: pageSize);

        Assert.Equal(expectedOffset, query.Offset);
    }
}
