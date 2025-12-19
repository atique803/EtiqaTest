using Dapper;
using PayrollSystem.Domain.Entities;
using PayrollSystem.Domain.Interfaces;

namespace PayrollSystem.Infrastructure.Repositories;

public class SkillsetRepository : ISkillsetRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SkillsetRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Skillset?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM Skillsets WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Skillset>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Skillset>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM Skillsets ORDER BY Name";
        return await connection.QueryAsync<Skillset>(sql);
    }

    public async Task<Skillset?> GetByNameAsync(string name)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT * FROM Skillsets WHERE Name = @Name";
        return await connection.QuerySingleOrDefaultAsync<Skillset>(sql, new { Name = name });
    }

    public async Task<int> CreateAsync(Skillset skillset)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO Skillsets (Name, Description, CreatedAt)
            VALUES (@Name, @Description, @CreatedAt);
            
            SELECT CAST(SCOPE_IDENTITY() as int);";
        
        skillset.CreatedAt = DateTime.UtcNow;
        
        return await connection.ExecuteScalarAsync<int>(sql, skillset);
    }

    public async Task<bool> UpdateAsync(Skillset skillset)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            UPDATE Skillsets 
            SET Name = @Name,
                Description = @Description
            WHERE Id = @Id";
        
        var rowsAffected = await connection.ExecuteAsync(sql, skillset);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "DELETE FROM Skillsets WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
