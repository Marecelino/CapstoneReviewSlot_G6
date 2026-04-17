using Availability.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Availability.Infrastructure.Persistence;

public class AvailabilityDbContext : DbContext
{
    public AvailabilityDbContext(DbContextOptions<AvailabilityDbContext> options) : base(options) { }

    public DbSet<LecturerAvailability> LecturerAvailabilities => Set<LecturerAvailability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LecturerAvailability>(e =>
        {
            e.ToTable("LecturerAvailability");
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasDefaultValueSql("NEWID()");
            e.Property(a => a.LecturerId).IsRequired();
            e.Property(a => a.ReviewSlotId).IsRequired();
            e.Property(a => a.Status).IsRequired().HasConversion<string>().HasMaxLength(30);
            e.Property(a => a.RegisteredAt).IsRequired();

            // UQ: (LecturerId, ReviewSlotId) — 1 lecturer chỉ đăng ký mỗi slot 1 lần
            e.HasIndex(a => new { a.LecturerId, a.ReviewSlotId }).IsUnique();

            // Performance indexes
            e.HasIndex(a => a.LecturerId);
            e.HasIndex(a => a.ReviewSlotId);
        });
    }
}
