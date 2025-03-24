namespace SchedlifyApi.DTO;

public class GroupBase
{
    public string Name { get; set; }
    public int DepartmentId { get; set; }
}

public class GroupResponse: GroupBase
{
    public int Id { get; set; }
}

public class CreateGroupRequest : GroupBase
{
}