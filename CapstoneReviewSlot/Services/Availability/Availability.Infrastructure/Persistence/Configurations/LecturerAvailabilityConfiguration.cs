using Availability.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Infrastructure.Persistence.Configurations
{
    public class LecturerAvailabilityConfiguration : IEntityTypeConfiguration<LecturerAvailability>
    {
        public void Configure(EntityTypeBuilder<LecturerAvailability> builder)
        {
            builder.ToTable("LecturerAvailability");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("AvailabilityId");

            builder.Property(x => x.LecturerId)
                .IsRequired();

            builder.Property(x => x.ReviewSlotId)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Không cho cùng một giảng viên đăng ký trùng một slot nhiều lần
            builder.HasIndex(x => new { x.LecturerId, x.ReviewSlotId })
                .IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }

}
