using Assignment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Data
{
    public class CapstoneReviewDBContext : DbContext
    {
        public CapstoneReviewDBContext(DbContextOptions<CapstoneReviewDBContext> options) : base(options)
        {
        }

        public DbSet<ReviewCampaign> ReviewCampaigns => Set<ReviewCampaign>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the ReviewCampaign entity
            modelBuilder.Entity<ReviewCampaign>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.MaxGroupsPerLecturer).IsRequired();
                entity.Property(e => e.RequiredReviewersPerGroup).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            });
        }
    }
}
