using Assignment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Persistence.Configurations
{
    public class ReviewAssignmentConfiguration : IEntityTypeConfiguration<ReviewAssignment>
    {
        public void Configure(EntityTypeBuilder<ReviewAssignment> builder)
        {
            builder.ToTable("ReviewAssignment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ReviewAssignmentId");

            builder.Property(x => x.CapstoneGroupId)
                .IsRequired();

            builder.Property(x => x.ReviewSlotId)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ReviewOrder)
                .IsRequired();

            builder.Property(x => x.AssignedBy)
                .IsRequired();

            builder.Property(x => x.AssignedAt)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Không cho cùng một group bị assign trùng nhiều lần trong cùng một slot
            builder.HasIndex(x => new { x.CapstoneGroupId, x.ReviewSlotId })
                .IsUnique();

            // Không cho trùng thứ tự review trong cùng một slot
            builder.HasIndex(x => new { x.ReviewSlotId, x.ReviewOrder })
                .IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }

}
