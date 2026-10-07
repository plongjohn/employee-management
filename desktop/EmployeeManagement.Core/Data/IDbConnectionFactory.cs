using System.Data.Common;

namespace EmployeeManagement.Core.Data;

public interface IDbConnectionFactory
{
    Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken);
}
