using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Session.Domain.Entities;

namespace Session.Infrastructure.Persistence.Configurations;

public class CapstoneGroupConfiguration : IEntityTypeConfiguration<CapstoneGroup>
{
    public void Configure(EntityTypeBuilder<CapstoneGroup> builder)
    {
        builder.ToTable("CapstoneGroup");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("CapstoneGroupId");

        builder.Property(x => x.CampaignId).IsRequired();
        builder.Property(x => x.GroupCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProjectCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ProjectNameEn).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ProjectNameVn).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.MentorLecturerId).IsRequired();
        builder.Property(x => x.SupervisorJson).IsRequired().HasMaxLength(500);

        builder.HasOne(x => x.ReviewCampaign)
            .WithMany()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Members)
            .WithOne(m => m.CapstoneGroup)
            .HasForeignKey(m => m.CapstoneGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
