using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Lecturer> Lecturers => Set<Lecturer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("User");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("UserId");
            e.Property(u => u.Email).IsRequired().HasMaxLength(255);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            e.Property(u => u.Role).IsRequired().HasConversion<string>().HasMaxLength(50);
            e.Property(u => u.Status).IsRequired().HasConversion<string>().HasMaxLength(30);
            e.Property(u => u.PasswordHash).IsRequired();
        });

        // Lecturer
        modelBuilder.Entity<Lecturer>(e =>
        {
            e.ToTable("Lecturer");
            e.HasKey(l => l.LecturerId);
            e.Property(l => l.LecturerId).UseIdentityColumn();
            e.Property(l => l.LecturerCode).IsRequired().HasMaxLength(50);
            e.HasIndex(l => l.LecturerCode).IsUnique();
            e.HasIndex(l => l.UserId).IsUnique();
            e.Property(l => l.Department).HasMaxLength(100);

            e.HasOne(l => l.User)
             .WithOne(u => u.Lecturer)
             .HasForeignKey<Lecturer>(l => l.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
