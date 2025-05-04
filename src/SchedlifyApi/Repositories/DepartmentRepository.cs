using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Data;
using SchedlifyApi.Models;


namespace SchedlifyApi.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public DepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetById(int departmentId)
    {
        return await _context.Departments.FindAsync(departmentId);
    }

    public async Task<List<Department>> GetAll(int universityId)
    {
        return await _context.Departments
            .Where(d => d.UniversityId == universityId)
            .ToListAsync();
    }

    public async Task<List<Department>> GetAll(
        int universityId, string? s, int offset = 0, int limit = 10)
    {
        IQueryable<Department> query = _context.Departments.Where(d => d.UniversityId == universityId);

        if (!string.IsNullOrEmpty(s))
        {
            query = query.Where(u => u.Name.Contains(s));
        }

        return await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Department?> GetByName(string name)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<List<Department>> GetByNamePart(string name)
    {
        return await _context.Departments
            .Where(u => u.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<Department> Create(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Department> Update(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task Delete(int departmentId)
    {
        var department = await _context.Departments.FindAsync(departmentId);
        if (department != null)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}