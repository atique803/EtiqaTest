using Dapper;
using PayrollSystem.Application.DTOs;
using PayrollSystem.Application.Interfaces;
using PayrollSystem.Domain.Entities;
using PayrollSystem.Domain.Interfaces;
using PayrollSystem.Domain.ValueObjects;

namespace PayrollSystem.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISkillsetRepository _skillsetRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        ISkillsetRepository skillsetRepository,
        IDbConnectionFactory connectionFactory)
    {
        _employeeRepository = employeeRepository;
        _skillsetRepository = skillsetRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        // Generate employee number
        var employeeNumber = EmployeeNumberGenerator.Generate(dto.EmployeeName, dto.DateOfBirth);

        var employee = new Employee
        {
            EmployeeNumber = employeeNumber,
            EmployeeName = dto.EmployeeName,
            NationalIdentificationNumber = dto.NationalIdentificationNumber,
            ContactNumber = dto.ContactNumber,
            ResidenceAddress = dto.ResidenceAddress,
            DateOfBirth = dto.DateOfBirth,
            DailyRate = dto.DailyRate,
            IsArchived = false
        };

        var employeeId = await _employeeRepository.CreateAsync(employee);
        employee.Id = employeeId;

        // Add skillsets
        if (dto.SkillsetIds.Any())
        {
            using var connection = _connectionFactory.CreateConnection();
            foreach (var skillsetId in dto.SkillsetIds)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO EmployeeSkillsets (EmployeeId, SkillsetId, AssignedAt) VALUES (@EmployeeId, @SkillsetId, @AssignedAt)",
                    new { EmployeeId = employeeId, SkillsetId = skillsetId, AssignedAt = DateTime.UtcNow });
            }
        }

        // Add working days
        if (dto.WorkingDays.Any())
        {
            using var connection = _connectionFactory.CreateConnection();
            foreach (var day in dto.WorkingDays)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO EmployeeWorkingDays (EmployeeId, DayOfWeek, CreatedAt) VALUES (@EmployeeId, @DayOfWeek, @CreatedAt)",
                    new { EmployeeId = employeeId, DayOfWeek = (int)day, CreatedAt = DateTime.UtcNow });
            }
        }

        return await GetEmployeeByIdAsync(employeeId) ?? throw new Exception("Failed to create employee");
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        return employee == null ? null : await MapToDto(employee);
    }

    public async Task<EmployeeDto?> GetEmployeeByNumberAsync(string employeeNumber)
    {
        var employee = await _employeeRepository.GetByEmployeeNumberAsync(employeeNumber);
        return employee == null ? null : await MapToDto(employee);
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(bool includeArchived = false)
    {
        var employees = await _employeeRepository.GetEmployeesWithDetailsAsync(includeArchived);
        var dtos = new List<EmployeeDto>();
        
        foreach (var employee in employees)
        {
            dtos.Add(await MapToDto(employee));
        }
        
        return dtos;
    }

    public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm, bool includeArchived = false)
    {
        var employees = await _employeeRepository.SearchAsync(searchTerm, includeArchived);
        var dtos = new List<EmployeeDto>();
        
        foreach (var employee in employees)
        {
            var fullEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
            if (fullEmployee != null)
            {
                dtos.Add(await MapToDto(fullEmployee));
            }
        }
        
        return dtos;
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(UpdateEmployeeDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(dto.Id);
        if (employee == null)
            throw new Exception("Employee not found");

        employee.EmployeeName = dto.EmployeeName;
        employee.NationalIdentificationNumber = dto.NationalIdentificationNumber;
        employee.ContactNumber = dto.ContactNumber;
        employee.ResidenceAddress = dto.ResidenceAddress;
        employee.DateOfBirth = dto.DateOfBirth;
        employee.DailyRate = dto.DailyRate;

        await _employeeRepository.UpdateAsync(employee);

        // Update skillsets
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync("DELETE FROM EmployeeSkillsets WHERE EmployeeId = @EmployeeId", new { EmployeeId = dto.Id });
        
        foreach (var skillsetId in dto.SkillsetIds)
        {
            await connection.ExecuteAsync(
                "INSERT INTO EmployeeSkillsets (EmployeeId, SkillsetId, AssignedAt) VALUES (@EmployeeId, @SkillsetId, @AssignedAt)",
                new { EmployeeId = dto.Id, SkillsetId = skillsetId, AssignedAt = DateTime.UtcNow });
        }

        // Update working days
        await connection.ExecuteAsync("DELETE FROM EmployeeWorkingDays WHERE EmployeeId = @EmployeeId", new { EmployeeId = dto.Id });
        
        foreach (var day in dto.WorkingDays)
        {
            await connection.ExecuteAsync(
                "INSERT INTO EmployeeWorkingDays (EmployeeId, DayOfWeek, CreatedAt) VALUES (@EmployeeId, @DayOfWeek, @CreatedAt)",
                new { EmployeeId = dto.Id, DayOfWeek = (int)day, CreatedAt = DateTime.UtcNow });
        }

        return await GetEmployeeByIdAsync(dto.Id) ?? throw new Exception("Failed to update employee");
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        return await _employeeRepository.DeleteAsync(id);
    }

    public async Task<bool> ArchiveEmployeeAsync(int id)
    {
        return await _employeeRepository.ArchiveAsync(id);
    }

    public async Task<bool> UnarchiveEmployeeAsync(int id)
    {
        return await _employeeRepository.UnarchiveAsync(id);
    }

    public async Task<CalculateSalaryResponse> CalculateSalaryAsync(CalculateSalaryRequest request)
    {
        var employee = await _employeeRepository.GetByEmployeeNumberAsync(request.EmployeeNumber);
        if (employee == null)
            throw new Exception("Employee not found");

        var workingDays = employee.EmployeeWorkingDays.Select(wd => wd.DayOfWeek).ToList();
        
        // Count working days in the period
        int workingDaysCount = 0;
        for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
        {
            if (workingDays.Contains(date.DayOfWeek))
            {
                workingDaysCount++;
            }
        }

        // Check if birthday falls in the period
        int birthdayBonus = 0;
        string? birthdayDate = null;
        
        for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
        {
            if (date.Month == employee.DateOfBirth.Month && date.Day == employee.DateOfBirth.Day)
            {
                birthdayBonus = 1;
                birthdayDate = date.ToString("MMM dd, yyyy");
                break;
            }
        }

        // Calculate salary: working days * 2 * daily rate + birthday bonus * daily rate * 2
        var totalDaysPaid = workingDaysCount + birthdayBonus;
        var takeHomePay = (workingDaysCount * 2 * employee.DailyRate) + (birthdayBonus * employee.DailyRate * 2);

        return new CalculateSalaryResponse
        {
            EmployeeNumber = employee.EmployeeNumber,
            EmployeeName = employee.EmployeeName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DailyRate = employee.DailyRate,
            TotalWorkingDays = workingDaysCount,
            BirthdayBonus = birthdayBonus,
            TotalDaysPaid = totalDaysPaid,
            TakeHomePay = takeHomePay,
            WorkingDaysList = workingDays.Select(d => d.ToString()).ToList(),
            BirthdayDate = birthdayDate
        };
    }

    private async Task<EmployeeDto> MapToDto(Employee employee)
    {
        var skillsets = new List<SkillsetDto>();
        
        using var connection = _connectionFactory.CreateConnection();
        var skillsetData = await connection.QueryAsync<dynamic>(
            @"SELECT s.Id, s.Name, s.Description 
              FROM Skillsets s
              INNER JOIN EmployeeSkillsets es ON s.Id = es.SkillsetId
              WHERE es.EmployeeId = @EmployeeId",
            new { EmployeeId = employee.Id });

        foreach (var skill in skillsetData)
        {
            skillsets.Add(new SkillsetDto
            {
                Id = skill.Id,
                Name = skill.Name,
                Description = skill.Description
            });
        }

        return new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            EmployeeName = employee.EmployeeName,
            NationalIdentificationNumber = employee.NationalIdentificationNumber,
            ContactNumber = employee.ContactNumber,
            ResidenceAddress = employee.ResidenceAddress,
            DateOfBirth = employee.DateOfBirth,
            DailyRate = employee.DailyRate,
            IsArchived = employee.IsArchived,
            Skillsets = skillsets,
            WorkingDays = employee.EmployeeWorkingDays.Select(wd => wd.DayOfWeek).ToList(),
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }
}
