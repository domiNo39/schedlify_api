using SchedlifyApi.Models;


namespace SchedlifyApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserById(int userId);

    User? GetByLogin(string login);

    User Add(User user);
}