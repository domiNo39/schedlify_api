namespace SchedlifyApi.DTO;

public class UserBase
{
    public string Login { get; set; }
}

public class RegisterUserRequest : UserBase
{
    public string Password { get; set; }
}

public class LoginUserRequest : UserBase
{
    public string Password { get; set; }
}

public class UserResponse : UserBase
{
    public int Id { get; set; }
}