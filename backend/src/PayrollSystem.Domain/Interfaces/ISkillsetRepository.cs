using PayrollSystem.Domain.Entities;

namespace PayrollSystem.Domain.Interfaces;

public interface ISkillsetRepository
{
    Task<Skillset?> GetByIdAsync(int id);
    Task<IEnumerable<Skillset>> GetAllAsync();
    Task<Skillset?> GetByNameAsync(string name);
    Task<int> CreateAsync(Skillset skillset);
    Task<bool> UpdateAsync(Skillset skillset);
    Task<bool> DeleteAsync(int id);
}
