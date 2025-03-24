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
    public DbSet<User> Users { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<TemplateSlot> TemplateSlots { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Class> Classes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TgUser>()
            .HasIndex(u => u.Username)  // Unused index. Needs refactoring
            .IsUnique();// User configuration
        
        // Legacy relations from schedlify_app below 
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Login).IsUnique();
        });

        // University configuration
        modelBuilder.Entity<University>(entity =>
        {
        });

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasOne(d => d.University)
                  .WithMany(u => u.Departments)
                  .HasForeignKey(d => d.UniversityId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Group configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasOne(g => g.Department)
                  .WithMany(d => d.Groups)
                  .HasForeignKey(g => g.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);

            // entity.HasOne(g => g.Administrator)
            //       .WithMany(u => u.AdministratedGroups)
            //       .HasForeignKey(g => g.AdministratorId)
            //       .OnDelete(DeleteBehavior.Restrict);
        });

        // TemplateSlot configuration
        modelBuilder.Entity<TemplateSlot>(entity =>
        {
            entity.HasOne(ts => ts.Department)
                  .WithMany(d => d.TemplateSlots)
                  .HasForeignKey(ts => ts.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Assignment configuration
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.Property(e => e.Type).HasColumnName("Type");
            entity.Property(e => e.Date).HasColumnName("Date");

            entity.HasOne(a => a.Group)
                  .WithMany(g => g.Assignments)
                  .HasForeignKey(a => a.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Class)
                  .WithMany(c => c.Assignments)
                  .HasForeignKey(a => a.ClassId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Class configuration
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasOne(c => c.Group)
                  .WithMany(g => g.Classes)
                  .HasForeignKey(c => c.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}