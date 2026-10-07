namespace EmployeeManagement.Core.Models;

public static class EmployeePageExtensions
{
    /// <summary>
    /// True when both pages show the same employees in the same order and version, so a list
    /// refreshed in the background has nothing to redraw. Every update increments the version,
    /// so id and version together reveal any change to a row.
    /// </summary>
    public static bool HasSameContentAs(this PagedResult<Employee> page, PagedResult<Employee> other) =>
        page.TotalCount == other.TotalCount
        && page.Page == other.Page
        && page.PageSize == other.PageSize
        && page.Items.Select(IdAndVersion).SequenceEqual(other.Items.Select(IdAndVersion));

    private static (int Id, uint Version) IdAndVersion(Employee employee) => (employee.Id, employee.Version);
}
