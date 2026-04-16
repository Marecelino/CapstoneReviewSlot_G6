using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Session.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session.Infrastructure.Persistence.Configurations
{
    public class ReviewSlotConfiguration : IEntityTypeConfiguration<ReviewSlot>
    {
        public void Configure(EntityTypeBuilder<ReviewSlot> builder)
        {
            builder.ToTable("ReviewSlot");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ReviewSlotId");

            builder.Property(x => x.CampaignId)
                .IsRequired();

            builder.Property(x => x.ReviewDate)
                .IsRequired();

            builder.Property(x => x.SlotNumber)
                .IsRequired();

            builder.Property(x => x.StartTime)
                .IsRequired();

            builder.Property(x => x.EndTime)
                .IsRequired();

            builder.Property(x => x.Room)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.MaxCapacity)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(x => x.ReviewCampaign)
                .WithMany(x => x.ReviewSlots)
                .HasForeignKey(x => x.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }

}
