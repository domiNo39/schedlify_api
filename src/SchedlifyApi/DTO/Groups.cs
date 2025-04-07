using SchedlifyApi.Models;

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

public class GroupExtendedResponse : GroupResponse
{
    public DepartmentResponse Department { get; set; }
    public UniversityResponse University { get; set; }
}


public class CreateGroupRequest : GroupBase
{
}