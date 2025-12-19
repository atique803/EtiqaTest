using PayrollSystem.Domain.Entities;

namespace PayrollSystem.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
    Task<IEnumerable<Employee>> GetAllAsync(bool includeArchived = false);
    Task<IEnumerable<Employee>> SearchAsync(string searchTerm, bool includeArchived = false);
    Task<int> CreateAsync(Employee employee);
    Task<bool> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(int id);
    Task<bool> ArchiveAsync(int id);
    Task<bool> UnarchiveAsync(int id);
    Task<IEnumerable<Employee>> GetEmployeesWithDetailsAsync(bool includeArchived = false);
}
