using SchedlifyApi.Models;

namespace SchedlifyApi.Data;

using Microsoft.EntityFrameworkCore;
using SchedlifyApi.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TgUser> TgUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TgUser>()
            .HasIndex(u => u.Username)  // Unused index. Needs refactoring
            .IsUnique();
    }
}