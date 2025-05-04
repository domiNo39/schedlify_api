using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Data;
using SchedlifyApi.Models;

namespace SchedlifyApi.Repositories;


public class UserRepository: IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetUserById(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }
    
    public User? GetByLogin(string login)
    {
        return _context.Users
            .FirstOrDefault(u => u.Login == login);  // Find the user by login
    }
    
    public User Add(User user)
    {

        _context.Users.Add(user);  // Add the user to the DbSet
        _context.SaveChanges();    // Save changes to the database

        return user;  // Return the added user
    }
}