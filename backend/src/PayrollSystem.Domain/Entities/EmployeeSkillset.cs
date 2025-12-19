namespace PayrollSystem.Domain.Entities;

public class EmployeeSkillset
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int SkillsetId { get; set; }
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;
    public Skillset Skillset { get; set; } = null!;
}
