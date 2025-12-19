using Dapper;
using PayrollSystem.Domain.Entities;
using PayrollSystem.Domain.Interfaces;

namespace PayrollSystem.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmployeeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT * FROM Employees WHERE Id = @Id;
            SELECT * FROM EmployeeSkillsets WHERE EmployeeId = @Id;
            SELECT * FROM EmployeeWorkingDays WHERE EmployeeId = @Id;";

        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });
        
        var employee = await multi.ReadSingleOrDefaultAsync<Employee>();
        if (employee != null)
        {
            var skillsets = await multi.ReadAsync<EmployeeSkillset>();
            var workingDays = await multi.ReadAsync<EmployeeWorkingDay>();
            
            employee.EmployeeSkillsets = skillsets.ToList();
            employee.EmployeeWorkingDays = workingDays.ToList();
        }
        
        return employee;
    }

    public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT * FROM Employees WHERE EmployeeNumber = @EmployeeNumber;
            SELECT es.* FROM EmployeeSkillsets es 
            INNER JOIN Employees e ON es.EmployeeId = e.Id 
            WHERE e.EmployeeNumber = @EmployeeNumber;
            SELECT wd.* FROM EmployeeWorkingDays wd 
            INNER JOIN Employees e ON wd.EmployeeId = e.Id 
            WHERE e.EmployeeNumber = @EmployeeNumber;";

        using var multi = await connection.QueryMultipleAsync(sql, new { EmployeeNumber = employeeNumber });
        
        var employee = await multi.ReadSingleOrDefaultAsync<Employee>();
        if (employee != null)
        {
            var skillsets = await multi.ReadAsync<EmployeeSkillset>();
            var workingDays = await multi.ReadAsync<EmployeeWorkingDay>();
            
            employee.EmployeeSkillsets = skillsets.ToList();
            employee.EmployeeWorkingDays = workingDays.ToList();
        }
        
        return employee;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(bool includeArchived = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = includeArchived 
            ? "SELECT * FROM Employees" 
            : "SELECT * FROM Employees WHERE IsArchived = 0";
        
        return await connection.QueryAsync<Employee>(sql);
    }

    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm, bool includeArchived = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT * FROM Employees 
            WHERE (EmployeeNumber LIKE @SearchTerm OR EmployeeName LIKE @SearchTerm)";
        
        if (!includeArchived)
        {
            sql += " AND IsArchived = 0";
        }
        
        var parameter = new { SearchTerm = $"%{searchTerm}%" };
        return await connection.QueryAsync<Employee>(sql, parameter);
    }

    public async Task<int> CreateAsync(Employee employee)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO Employees 
            (EmployeeNumber, EmployeeName, NationalIdentificationNumber, ContactNumber, 
             ResidenceAddress, DateOfBirth, DailyRate, IsArchived, CreatedAt, UpdatedAt)
            VALUES 
            (@EmployeeNumber, @EmployeeName, @NationalIdentificationNumber, @ContactNumber, 
             @ResidenceAddress, @DateOfBirth, @DailyRate, @IsArchived, @CreatedAt, @UpdatedAt);
            
            SELECT CAST(SCOPE_IDENTITY() as int);";
        
        employee.CreatedAt = DateTime.UtcNow;
        employee.UpdatedAt = DateTime.UtcNow;
        
        return await connection.ExecuteScalarAsync<int>(sql, employee);
    }

    public async Task<bool> UpdateAsync(Employee employee)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            UPDATE Employees 
            SET EmployeeName = @EmployeeName,
                NationalIdentificationNumber = @NationalIdentificationNumber,
                ContactNumber = @ContactNumber,
                ResidenceAddress = @ResidenceAddress,
                DateOfBirth = @DateOfBirth,
                DailyRate = @DailyRate,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";
        
        employee.UpdatedAt = DateTime.UtcNow;
        
        var rowsAffected = await connection.ExecuteAsync(sql, employee);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "DELETE FROM Employees WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "UPDATE Employees SET IsArchived = 1, UpdatedAt = @UpdatedAt WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }

    public async Task<bool> UnarchiveAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "UPDATE Employees SET IsArchived = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesWithDetailsAsync(bool includeArchived = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = includeArchived 
            ? "SELECT * FROM Employees" 
            : "SELECT * FROM Employees WHERE IsArchived = 0";
        
        var employees = (await connection.QueryAsync<Employee>(sql)).ToList();
        
        if (employees.Any())
        {
            var employeeIds = employees.Select(e => e.Id).ToList();
            
            var skillsetsSql = "SELECT * FROM EmployeeSkillsets WHERE EmployeeId IN @EmployeeIds";
            var workingDaysSql = "SELECT * FROM EmployeeWorkingDays WHERE EmployeeId IN @EmployeeIds";
            
            var skillsets = (await connection.QueryAsync<EmployeeSkillset>(skillsetsSql, new { EmployeeIds = employeeIds }))
                .GroupBy(es => es.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            var workingDays = (await connection.QueryAsync<EmployeeWorkingDay>(workingDaysSql, new { EmployeeIds = employeeIds }))
                .GroupBy(wd => wd.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var employee in employees)
            {
                employee.EmployeeSkillsets = skillsets.GetValueOrDefault(employee.Id) ?? new List<EmployeeSkillset>();
                employee.EmployeeWorkingDays = workingDays.GetValueOrDefault(employee.Id) ?? new List<EmployeeWorkingDay>();
            }
        }
        
        return employees;
    }
}
