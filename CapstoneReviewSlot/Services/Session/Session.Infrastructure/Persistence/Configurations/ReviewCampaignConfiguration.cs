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
    public class ReviewCampaignConfiguration : IEntityTypeConfiguration<ReviewCampaign>
    {
        public void Configure(EntityTypeBuilder<ReviewCampaign> builder)
        {
            builder.ToTable("ReviewCampaign");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("CampaignId");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.StartTime)
                .IsRequired();

            builder.Property(x => x.EndTime)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }

}
