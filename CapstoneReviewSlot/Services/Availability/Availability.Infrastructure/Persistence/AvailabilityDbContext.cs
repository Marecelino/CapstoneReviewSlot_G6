using Availability.Domain.Entities;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Infrastructure.Persistence
{
    public class AvailabilityDbContext : DbContext
    {
        public AvailabilityDbContext(DbContextOptions<AvailabilityDbContext> options)
            : base(options)
        {
        }

        public DbSet<LecturerAvailability> LecturerAvailabilities => Set<LecturerAvailability>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AvailabilityDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.Id == Guid.Empty)
                        entry.Entity.Id = Guid.NewGuid();

                    if (entry.Entity.CreatedAtUtc == default)
                        entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }

}
