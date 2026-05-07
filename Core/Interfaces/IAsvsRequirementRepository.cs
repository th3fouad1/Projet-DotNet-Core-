using ASVS_Security_Auditor.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASVS_Security_Auditor.Models;

namespace ASVS_Security_Auditor.Core.Interfaces;

public interface IAsvsRequirementRepository
{
    Task<IEnumerable<AsvsRequirement>> GetAllAsync();
    Task<AsvsRequirement?> GetByIdAsync(int id);
    Task AddAsync(AsvsRequirement requirement);
    Task UpdateAsync(AsvsRequirement requirement);
    Task DeleteAsync(AsvsRequirement requirement);
    Task<int> CountAsync();
}


