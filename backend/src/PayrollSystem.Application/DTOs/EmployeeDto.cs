namespace PayrollSystem.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string NationalIdentificationNumber { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string ResidenceAddress { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public decimal DailyRate { get; set; }
    public bool IsArchived { get; set; }
    public List<SkillsetDto> Skillsets { get; set; } = new();
    public List<DayOfWeek> WorkingDays { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SkillsetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
