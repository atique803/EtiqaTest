namespace PayrollSystem.Domain.Entities;

public class EmployeeWorkingDay
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;
}
