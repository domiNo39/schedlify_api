using SchedlifyApi.Models;

namespace SchedlifyApi.Repositories;
using SchedlifyApi.Models;

public interface ITgUserRepository
{
    Task<TgUser> CreateAsync(TgUser user);
    Task<TgUser> GetByIdAsync(long id);
    Task<TgUser> GetByUsernameAsync(string username);
    Task<List<TgUser>> GetAllAsync();
    Task<TgUser> UpdateAsync(TgUser user);
    Task DeleteAsync(long id);
}