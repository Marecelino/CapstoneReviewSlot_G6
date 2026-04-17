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
    public class ReviewAssignmentReviewerConfiguration : IEntityTypeConfiguration<ReviewAssignmentReviewer>
    {
        public void Configure(EntityTypeBuilder<ReviewAssignmentReviewer> builder)
        {
            builder.ToTable("ReviewAssignmentReviewer");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ReviewerId");

            builder.Property(x => x.LecturerId)
                .IsRequired();

            builder.Property(x => x.ReviewAssignmentId)
                .IsRequired();

            builder.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(x => x.ReviewAssignment)
                .WithMany(x => x.Reviewers)
                .HasForeignKey(x => x.ReviewAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Không cho cùng một lecturer bị gán trùng trong cùng một assignment
            builder.HasIndex(x => new { x.LecturerId, x.ReviewAssignmentId })
                .IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }

}
