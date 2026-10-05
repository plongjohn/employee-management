using Dapper;
using EmployeeManagement.Core.Data;
using EmployeeManagement.Core.Models;

namespace EmployeeManagement.Core.Repositories;

public sealed class DepartmentRepository(IDbConnectionFactory connectionFactory) : IDepartmentRepository
{
    private const string SelectAllSql = "SELECT id AS Id, name AS Name FROM departments ORDER BY name";

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        var departments = await connection.QueryAsync<Department>(
            new CommandDefinition(SelectAllSql, cancellationToken: cancellationToken));

        return departments.AsList();
    }
}
