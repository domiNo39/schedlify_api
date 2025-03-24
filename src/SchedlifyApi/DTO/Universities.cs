namespace SchedlifyApi.DTO;

public class UniversityBase
{
    public string Name { get; set; }
}

public class CreateUniversityRequest : UniversityBase
{
    
}

public class UniversityResponse : UniversityBase
{
    public int Id { get; set; }
}