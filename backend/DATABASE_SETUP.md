# Database Setup Guide

This project uses **Dapper** for database interaction and **DbUp** for database migrations.

## Database Structure

### Entities Created

1. **Employees** - Main employee information
   - Id, EmployeeNumber, EmployeeName, NationalIdentificationNumber
   - ContactNumber, ResidenceAddress, DateOfBirth
   - DailyRate, IsArchived, CreatedAt, UpdatedAt

2. **Skillsets** - Available skills in the system
   - Id, Name, Description, CreatedAt

3. **EmployeeSkillsets** - Junction table for many-to-many relationship
   - Id, EmployeeId, SkillsetId, AssignedAt

4. **EmployeeWorkingDays** - Days of the week an employee works
   - Id, EmployeeId, DayOfWeek (0=Sunday, 6=Saturday), CreatedAt

## Connection String Configuration

The connection string is configured in `appsettings.json` and `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PayrollSystemDb;User Id=sa;Password=Password123;TrustServerCertificate=True;"
  }
}
```

**Important:** Update the connection string with your actual SQL Server credentials.

## Running Migrations

Migrations run **automatically** when the application starts. The migration system:
- Creates the database if it doesn't exist
- Runs all pending migration scripts in order
- Keeps track of executed scripts to avoid re-running them

### Migration Files Location

Migration scripts are located in:
```
PayrollSystem.Infrastructure/Migrations/
├── 001_CreateEmployeesTable.sql
├── 002_CreateSkillsetsTable.sql
├── 003_CreateEmployeeSkillsetsTable.sql
├── 004_CreateEmployeeWorkingDaysTable.sql
└── 005_SeedSkillsets.sql
```

## Using SQL Server with Docker

If you don't have SQL Server installed, you can run it using Docker:

```bash
# Pull SQL Server image
docker pull mcr.microsoft.com/mssql/server:2022-latest

# Run SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

## Dapper Repository Pattern

The project uses the Repository pattern with Dapper:

### Repository Interfaces (Domain Layer)
- `IEmployeeRepository` - Employee data operations
- `ISkillsetRepository` - Skillset data operations
- `IDbConnectionFactory` - Database connection factory

### Repository Implementations (Infrastructure Layer)
- `EmployeeRepository` - Dapper-based employee operations
- `SkillsetRepository` - Dapper-based skillset operations
- `DbConnectionFactory` - SQL Server connection factory

### Key Features

1. **Raw SQL Queries** - Full control over SQL with Dapper
2. **No Entity Framework** - As per requirements (strictly Dapper)
3. **Clean Architecture** - Separation of concerns
4. **Automatic Migrations** - DbUp handles schema changes
5. **Async Operations** - All repository methods are async

## Example Usage

```csharp
// Inject repository in your service/controller
public class EmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Employee?> GetEmployeeAsync(int id)
    {
        return await _employeeRepository.GetByIdAsync(id);
    }
}
```

## Adding New Migrations

To add a new migration:

1. Create a new SQL file in `Infrastructure/Migrations/`
2. Name it with a sequential number: `006_YourMigrationName.sql`
3. The file will be automatically embedded and executed on next run

Example:
```sql
-- 006_AddNewColumn.sql
ALTER TABLE Employees ADD NewColumn NVARCHAR(100) NULL;
```

## Troubleshooting

### Migration Fails
- Check SQL Server is running: `docker ps` or check Windows Services
- Verify connection string credentials
- Check migration script syntax

### Cannot Connect to Database
- Ensure SQL Server port 1433 is accessible
- Verify firewall settings
- Check `TrustServerCertificate=True` is in connection string

### Scripts Not Running
- Ensure .csproj has: `<EmbeddedResource Include="Migrations\*.sql" />`
- Rebuild the Infrastructure project
- Check script file properties (should be Embedded Resource)

## NuGet Packages Used

- **Dapper** (2.1.66) - Micro ORM for data access
- **Microsoft.Data.SqlClient** (6.1.3) - SQL Server provider
- **DbUp** (5.0.41) - Database migration tool
