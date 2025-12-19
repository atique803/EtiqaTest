namespace PayrollSystem.Domain.Entities;

public class Skillset
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<EmployeeSkillset> EmployeeSkillsets { get; set; } = new List<EmployeeSkillset>();
}
