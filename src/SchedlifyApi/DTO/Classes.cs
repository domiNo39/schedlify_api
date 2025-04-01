namespace SchedlifyApi.DTO;

public class ClassBase
{
    public string Name { get; set; }
    public int GroupId { get; set; }
}

public class CreateClassRequest : ClassBase
{
    
}


public class EditClassRequest : ClassBase
{
    
}

public class ClassResponse : ClassBase
{
    public int Id { get; set; }
}