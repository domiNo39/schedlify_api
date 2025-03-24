namespace SchedlifyApi.DTO
{

    public class TgUserBase
    {
        public string? Username { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
    }
    public class CreateTgUserRequest: TgUserBase
    {
    }

    public class TgUserResponse: CreateTgUserRequest
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}