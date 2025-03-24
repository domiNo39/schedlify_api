namespace SchedlifyApi.DTO;

public class DepartmentBase
{
    public string Name { get; set; }
    public int UniversityId { get; set; }
}

public class CreateDepartmentResponse : DepartmentBase
{
    
}

public class DepartmentResponse : DepartmentBase
{
    public int Id { get; set; }
}