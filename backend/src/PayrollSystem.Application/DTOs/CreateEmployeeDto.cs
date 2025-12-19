namespace PayrollSystem.Application.DTOs;

public class CreateEmployeeDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string NationalIdentificationNumber { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string ResidenceAddress { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public decimal DailyRate { get; set; }
    public List<int> SkillsetIds { get; set; } = new();
    public List<DayOfWeek> WorkingDays { get; set; } = new();
}
