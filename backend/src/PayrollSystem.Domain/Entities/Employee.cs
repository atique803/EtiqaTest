namespace PayrollSystem.Domain.Entities;

public class Employee
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<EmployeeSkillset> EmployeeSkillsets { get; set; } = new List<EmployeeSkillset>();
    public ICollection<EmployeeWorkingDay> EmployeeWorkingDays { get; set; } = new List<EmployeeWorkingDay>();
}
