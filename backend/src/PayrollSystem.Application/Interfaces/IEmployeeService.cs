using PayrollSystem.Application.DTOs;

namespace PayrollSystem.Application.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
    Task<EmployeeDto?> GetEmployeeByNumberAsync(string employeeNumber);
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(bool includeArchived = false);
    Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm, bool includeArchived = false);
    Task<EmployeeDto> UpdateEmployeeAsync(UpdateEmployeeDto dto);
    Task<bool> DeleteEmployeeAsync(int id);
    Task<bool> ArchiveEmployeeAsync(int id);
    Task<bool> UnarchiveEmployeeAsync(int id);
    Task<CalculateSalaryResponse> CalculateSalaryAsync(CalculateSalaryRequest request);
}
