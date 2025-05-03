namespace SchedlifyApi.Repositories;

using SchedlifyApi.Models;

public interface IAssignmentRepository
{
    Task<List<Assignment>> GetAssignmentsByWeekday(int groupId, Weekday weekday, AssignmentType assignmentType);

    Task<List<Assignment>> GetByFilters(int? universityId, int? departmentId);
    Task<Assignment?> GetByIdAsync(int id);
    Task<List<Assignment>> GetAssignmentsByDate(int groupId, DateOnly date);

    Task<Assignment> AddAssignment(Assignment assignment);

    Task<Assignment> EditAssignment(Assignment assignment);

    Task DeleteAssignment(int assignmentId);
}