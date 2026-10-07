namespace EmployeeManagement.Core.Models;

public sealed record EmployeeQuery(
    string? SearchText = null,
    int? DepartmentId = null,
    EmployeeSortColumn SortBy = EmployeeSortColumn.Name,
    SortDirection Direction = SortDirection.Ascending,
    int Page = 1,
    int PageSize = EmployeeQuery.DefaultPageSize)
{
    public const int DefaultPageSize = 25;
    public const int MaxPageSize = 100;

    // Every term adds one OR group to the WHERE clause; the cap keeps the statement small
    // no matter what is typed. Five words are more than any name search needs.
    public const int MaxSearchTerms = 5;

    public IReadOnlyList<string> SearchTerms =>
        (SearchText ?? string.Empty)
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .Take(MaxSearchTerms)
            .ToArray();

    public int Offset => (Page - 1) * PageSize;
}
