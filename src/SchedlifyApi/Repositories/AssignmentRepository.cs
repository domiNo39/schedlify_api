using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Data;
using SchedlifyApi.Models;


namespace SchedlifyApi.Repositories;


public class AssignmentRepository: IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(int id)
    {
        return await _context.Assignments.FindAsync(id);
    }

    public async Task<List<Assignment>> GetAssignmentsByWeekday(int groupId, Weekday weekday, AssignmentType assignmentType)
    {
        return await _context.Assignments
            .Include(d => d.Class)
            .Where(a => a.GroupId == groupId && a.Weekday == weekday)
            .Where(a => a.Type == assignmentType && a.Date == null)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<List<Assignment>> GetAssignmentsByDate(int groupId, DateOnly date)
    {
        return await _context.Assignments
            .Include(d => d.Class)
            .Where(a => a.GroupId == groupId && a.Date == date)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }
    
    public async Task<Assignment> AddAssignment(Assignment assignment)
    {
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<Assignment> EditAssignment(Assignment assignment)
    {
        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task DeleteAssignment(int assignmentId)
    {
        var assignment = await _context.Assignments.FindAsync(assignmentId);
        if (assignment != null)
        {
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }
}