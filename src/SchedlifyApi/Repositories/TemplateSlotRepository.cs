//COPYRIGHT NIGGERCODE
namespace SchedlifyApi.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using SchedlifyApi.Data;

public class TemplateSlotRepository : ITemplateSlotRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TemplateSlotRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TemplateSlot>> GetByDepartmentIdAsync(int departmentId)
    {
        return await _dbContext.TemplateSlots
            .Where(ts => ts.DepartmentId == departmentId)
            .ToListAsync();
    }

    public async Task CreateBulkAsync(int departmentId, List<TemplateSlot> templateSlots)
    {
        foreach (var slot in templateSlots)
        {
            slot.DepartmentId = departmentId; 
        }
        await _dbContext.TemplateSlots.AddRangeAsync(templateSlots);
        await _dbContext.SaveChangesAsync();
    }
}