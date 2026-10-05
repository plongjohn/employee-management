namespace EmployeeManagement.Core.Data;

internal static class SqlLike
{
    /// <summary>
    /// Builds a LIKE pattern that matches values starting with <paramref name="term"/>.
    /// Wildcards typed by the user are escaped, so "50%" matches literally.
    /// A prefix pattern (no leading wildcard) lets MariaDB use the column index.
    /// </summary>
    public static string Prefix(string term)
    {
        // Backslash is MariaDB's default LIKE escape character, so it has to be escaped first.
        var escaped = term
            .Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_");

        return escaped + "%";
    }
}
