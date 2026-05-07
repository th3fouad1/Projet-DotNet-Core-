using ASVS_Security_Auditor.Core.Entities;
using ASVS_Security_Auditor.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ASVS_Security_Auditor.Models;
using Microsoft.EntityFrameworkCore;

namespace ASVS_Security_Auditor.Infrastructure.Data;

public class AsvsRequirementRepository : IAsvsRequirementRepository
{
    private readonly ApplicationDbContext _context;

    public AsvsRequirementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AsvsRequirement>> GetAllAsync()
    {
        return await _context.AsvsRequirements
            .OrderBy(r => r.ChapterId)
            .ThenBy(r => r.Item)
            .ToListAsync();
    }

    public async Task<AsvsRequirement?> GetByIdAsync(int id)
    {
        return await _context.AsvsRequirements.FindAsync(id);
    }

    public async Task AddAsync(AsvsRequirement requirement)
    {
        _context.AsvsRequirements.Add(requirement);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AsvsRequirement requirement)
    {
        _context.AsvsRequirements.Update(requirement);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AsvsRequirement requirement)
    {
        _context.AsvsRequirements.Remove(requirement);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.AsvsRequirements.CountAsync();
    }
}


