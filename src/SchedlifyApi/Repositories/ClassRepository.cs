using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Data;
using SchedlifyApi.Models;


namespace SchedlifyApi.Repositories;


public class ClassRepository: IClassRepository
{  
    private readonly ApplicationDbContext _context;

    public ClassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Class?> GetByIdAsync(int id)
    {
        return await _context.Classes
            .Include(c => c.Group) // Include related Group entity
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<List<Class>> GetByGroupId(int groupId)
    {
        return await _context.Classes
            .Include(c => c.Group)        
            .Where(c => c.GroupId == groupId).ToListAsync();
    }
    
    public async Task<Class> Create(Class department)
    {
        _context.Classes.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Class> Update(Class group)
    {
        _context.Classes.Update(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task Delete(int groupId)
    {
        var group = await _context.Classes.FindAsync(groupId);
        if (group != null)
        {
            _context.Classes.Remove(group);
            await _context.SaveChangesAsync();
        }
    }
}