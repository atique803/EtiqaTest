namespace PayrollSystem.Application.DTOs;

public class CalculateSalaryResponse
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DailyRate { get; set; }
    public int TotalWorkingDays { get; set; }
    public int BirthdayBonus { get; set; } // 1 if birthday falls in period, 0 otherwise
    public int TotalDaysPaid { get; set; }
    public decimal TakeHomePay { get; set; }
    public List<string> WorkingDaysList { get; set; } = new();
    public string? BirthdayDate { get; set; }
}
