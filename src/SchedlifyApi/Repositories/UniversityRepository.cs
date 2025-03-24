using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Data;
using SchedlifyApi.Models;


namespace SchedlifyApi.Repositories;

public class UniversityRepository : IUniversityRepository
{
    private readonly ApplicationDbContext _context;

    public UniversityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<University?> GetById(int universityId)
    {
        return await _context.Universities.FindAsync(universityId);
    }

    public async Task<List<University>> GetAll()
    {
        return await _context.Universities
            .ToListAsync();
    }

    public async Task<List<University>> GetAll(int offset, int limit)
    {
        return await _context.Universities
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<University?> GetByName(string name)
    {
        return await _context.Universities
            .FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<List<University>> GetByNamePart(string name)
    {
        return await _context.Universities
            .Where(u => u.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<University> Create(University university)
    {
        _context.Universities.Add(university);
        await _context.SaveChangesAsync();
        return university;
    }

    public async Task<University> Update(University university)
    {
        _context.Universities.Update(university);
        await _context.SaveChangesAsync();
        return university;
    }

    public async Task Delete(int universityId)
    {
        var university = await _context.Universities.FindAsync(universityId);
        if (university != null)
        {
            _context.Universities.Remove(university);
            await _context.SaveChangesAsync();
        }
    }
}
