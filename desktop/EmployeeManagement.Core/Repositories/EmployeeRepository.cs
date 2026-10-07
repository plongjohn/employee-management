using Dapper;
using EmployeeManagement.Core.Data;
using EmployeeManagement.Core.Models;
using MySqlConnector;

namespace EmployeeManagement.Core.Repositories;

public sealed class EmployeeRepository(IDbConnectionFactory connectionFactory) : IEmployeeRepository
{
    // Name of the unique index in database/setup.sql – change both together.
    private const string EmailUniqueIndex = "uq_employees_email";

    private const string SelectColumnsSql = """
        SELECT e.id AS Id, e.first_name AS FirstName, e.last_name AS LastName, e.email AS Email,
               e.department_id AS DepartmentId, d.name AS DepartmentName, e.hire_date AS HireDate,
               e.version AS Version, e.created_at AS CreatedAt, e.updated_at AS UpdatedAt
        FROM employees e
        JOIN departments d ON d.id = e.department_id
        """;

    private const string InsertSql = """
        INSERT INTO employees (first_name, last_name, email, department_id, hire_date)
        VALUES (@FirstName, @LastName, @Email, @DepartmentId, @HireDate)
        RETURNING id
        """;

    private const string UpdateSql = """
        UPDATE employees
        SET first_name = @FirstName, last_name = @LastName, email = @Email,
            department_id = @DepartmentId, hire_date = @HireDate, version = version + 1
        WHERE id = @Id AND version = @ExpectedVersion
        """;

    private const string DeleteSql = "DELETE FROM employees WHERE id = @Id AND version = @ExpectedVersion";

    public async Task<PagedResult<Employee>> SearchAsync(EmployeeQuery query, CancellationToken cancellationToken)
    {
        var (whereSql, parameters) = BuildFilter(query);
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("Offset", query.Offset);

        var sql = $"""
            {SelectColumnsSql}
            {whereSql}
            ORDER BY {BuildOrderBy(query.SortBy, query.Direction)}
            LIMIT @PageSize OFFSET @Offset;

            SELECT COUNT(*) FROM employees e {whereSql};
            """;

        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var results = await connection.QueryMultipleAsync(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        var employees = (await results.ReadAsync<Employee>()).AsList();
        var totalCount = await results.ReadSingleAsync<int>();

        return new PagedResult<Employee>(employees, totalCount, query.Page, query.PageSize);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Employee>(
            new CommandDefinition($"{SelectColumnsSql} WHERE e.id = @Id", new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<int> InsertAsync(EmployeeInput input, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        return await TranslateWriteErrors(() => connection.ExecuteScalarAsync<int>(
            new CommandDefinition(InsertSql, input, cancellationToken: cancellationToken)));
    }

    public async Task<bool> UpdateAsync(
        int id, uint expectedVersion, EmployeeInput input, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters(input);
        parameters.Add("Id", id);
        parameters.Add("ExpectedVersion", expectedVersion);

        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        var affectedRows = await TranslateWriteErrors(() => connection.ExecuteAsync(
            new CommandDefinition(UpdateSql, parameters, cancellationToken: cancellationToken)));

        return affectedRows == 1;
    }

    public async Task<bool> DeleteAsync(int id, uint expectedVersion, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(DeleteSql, new { Id = id, ExpectedVersion = expectedVersion },
                cancellationToken: cancellationToken));

        return affectedRows == 1;
    }

    private static (string WhereSql, DynamicParameters Parameters) BuildFilter(EmployeeQuery query)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        var searchTerms = query.SearchTerms;
        for (var i = 0; i < searchTerms.Count; i++)
        {
            var name = $"Term{i}";
            conditions.Add($"(e.first_name LIKE @{name} OR e.last_name LIKE @{name} OR e.email LIKE @{name})");
            parameters.Add(name, SqlLike.Prefix(searchTerms[i]));
        }

        // "All departments" means no condition at all: an "@id IS NULL OR ..." clause
        // would often keep MariaDB from using the department index.
        if (query.DepartmentId is { } departmentId)
        {
            conditions.Add("e.department_id = @DepartmentId");
            parameters.Add("DepartmentId", departmentId);
        }

        var whereSql = conditions.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", conditions);
        return (whereSql, parameters);
    }

    // Column names come only from this fixed mapping, never from user input.
    // e.id is the final tie-breaker so that rows with equal values keep a stable order across pages.
    // Email is unique and needs none; appending e.id there would stop MariaDB from reading
    // uq_employees_email in order and force a sort of the whole table.
    private static string BuildOrderBy(EmployeeSortColumn sortBy, SortDirection direction)
    {
        string[] columns = sortBy switch
        {
            EmployeeSortColumn.Name => ["e.last_name", "e.first_name", "e.id"],
            EmployeeSortColumn.Email => ["e.email"],
            EmployeeSortColumn.Department => ["d.name", "e.last_name", "e.first_name", "e.id"],
            EmployeeSortColumn.HireDate => ["e.hire_date", "e.id"],
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null),
        };

        var keyword = direction == SortDirection.Descending ? "DESC" : "ASC";
        return string.Join(", ", columns.Select(column => $"{column} {keyword}"));
    }

    private static async Task<T> TranslateWriteErrors<T>(Func<Task<T>> write)
    {
        try
        {
            return await write();
        }
        catch (MySqlException ex) when (IsDuplicateEmail(ex))
        {
            throw new DuplicateEmailException(ex);
        }
        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.NoReferencedRow2)
        {
            throw new DepartmentNotFoundException(ex);
        }
    }

    // The error code alone would also match any future unique index, so the index name is checked too.
    private static bool IsDuplicateEmail(MySqlException ex) =>
        ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry
        && ex.Message.Contains(EmailUniqueIndex, StringComparison.Ordinal);
}
