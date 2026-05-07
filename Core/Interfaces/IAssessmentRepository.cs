using ASVS_Security_Auditor.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASVS_Security_Auditor.Models;

namespace ASVS_Security_Auditor.Core.Interfaces;

public interface IAssessmentRepository
{
    Task<IEnumerable<Assessment>> GetAllWithItemsAsync();
    Task<Assessment?> GetByIdWithItemsAsync(int id);
    Task AddAsync(Assessment assessment);
    Task UpdateItemAsync(AssessmentItem item);
    Task<AssessmentItem?> GetItemByIdAsync(int itemId);
}


