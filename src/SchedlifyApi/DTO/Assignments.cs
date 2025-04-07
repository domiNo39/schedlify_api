namespace SchedlifyApi.DTO;

using SchedlifyApi.Models;

public class AssignmentBase
{
    public int ClassId { get; set; }
    public ClassType? ClassType { get; set; }
    public Mode? Mode { get; set; }
    public string? Lecturer { get; set; }
    public string? Address { get; set; }
    public string? RoomNumber { get; set; }
}

public class CreateRegularAssignmentRequest: AssignmentBase {
    public Weekday Weekday { get; set; }
    public int ClassNumber { get; set; }
}

public class CreateIntervalAssignmentRequest : AssignmentBase
{
    public Weekday Weekday { get; set; }
    public int ClassNumber { get; set; }
    public AssignmentType Type { get; set; }
}

public class CreateSpecialAssignmentRequest : AssignmentBase
{
    public DateOnly Date { get; set; }
    public int ClassNumber { get; set; }
}

public class AssignmentResponse : AssignmentBase
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public Weekday Weekday { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public DateOnly? Date { get; set; }
    
    public AssignmentType Type { get; set; }
}