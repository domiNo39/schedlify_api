using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Data;
using SchedlifyApi.Models;


namespace SchedlifyApi.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetById(int groupId)
    {
        return await _context.Groups.FindAsync(groupId);
    }

    public async Task<List<Group>> GetAll(int departmentId)
    {
        return await _context.Groups
            .Where(d => d.DepartmentId == departmentId)
            .ToListAsync();
    }

    public async Task<List<Group>> GetAll(
        int departmentId, string? s, int offset=0, int limit=10)
    {
        IQueryable<Group> query = _context.Groups.Where(d => d.DepartmentId == departmentId);
        if (!string.IsNullOrEmpty(s))
        {
            query = query.Where(u => u.Name.Contains(s));
        }

        return await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Group?> GetByName(string name)
    {
        return await _context.Groups
            .FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<List<Group>> GetByNamePart(string name)
    {
        return await _context.Groups
            .Where(u => u.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<Group> Create(Group department)
    {
        _context.Groups.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Group> Update(Group group)
    {
        _context.Groups.Update(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task Delete(int groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group != null)
        {
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
        }
    }
}