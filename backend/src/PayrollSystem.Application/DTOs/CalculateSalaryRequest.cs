namespace PayrollSystem.Application.DTOs;

public class CalculateSalaryRequest
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
