using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

using ASVS_Security_Auditor.Models;
using Microsoft.EntityFrameworkCore;

namespace ASVS_Security_Auditor.Infrastructure.Data;

public class AssessmentRepository : IAssessmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssessmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Assessment>> GetAllWithItemsAsync()
    {
        return await _context.Assessments
            .Include(a => a.Items)
            .ToListAsync();
    }

    public async Task<Assessment?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Assessments
            .Include(a => a.Items)
            .ThenInclude(i => i.AsvsRequirement)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Assessment assessment)
    {
        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(AssessmentItem item)
    {
        _context.AssessmentItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task<AssessmentItem?> GetItemByIdAsync(int itemId)
    {
        return await _context.AssessmentItems.FindAsync(itemId);
    }
}


