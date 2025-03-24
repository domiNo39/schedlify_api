using SchedlifyApi.Data;
using SchedlifyApi.Models;

using Microsoft.EntityFrameworkCore;

namespace SchedlifyApi.Repositories;

public class TgUserRepository : ITgUserRepository
{
    private readonly ApplicationDbContext _context;

    public TgUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TgUser> CreateAsync(TgUser user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.TgUsers.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<TgUser?> GetByIdAsync(long id)
    {
        return await _context.TgUsers.FindAsync(id);
    }

    public async Task<TgUser?> GetByUsernameAsync(string username)
    {
        return await _context.TgUsers
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<List<TgUser>> GetAllAsync()
    {
        return await _context.TgUsers.ToListAsync();
    }

    public async Task<TgUser> UpdateAsync(TgUser user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.TgUsers.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _context.TgUsers.FindAsync(id);
        if (user != null)
        {
            _context.TgUsers.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}