using System.Data;
using Microsoft.Data.SqlClient;

namespace PayrollSystem.Infrastructure.Data;

public class DbConnectionFactory : Domain.Interfaces.IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
