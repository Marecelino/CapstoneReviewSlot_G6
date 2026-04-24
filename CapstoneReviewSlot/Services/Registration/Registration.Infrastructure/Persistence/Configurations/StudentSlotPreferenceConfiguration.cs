using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Registration.Domain.Entities;

namespace Registration.Infrastructure.Persistence.Configurations;

public class StudentSlotPreferenceConfiguration : IEntityTypeConfiguration<StudentSlotPreference>
{
    public void Configure(EntityTypeBuilder<StudentSlotPreference> builder)
    {
        builder.ToTable("StudentSlotPreference");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("PreferenceId");

        builder.Property(x => x.CapstoneGroupId).IsRequired();
        builder.Property(x => x.ReviewSlotId).IsRequired();
        builder.Property(x => x.PreferenceOrder).IsRequired();
        builder.Property(x => x.StudentMssv).IsRequired().HasMaxLength(50);

        // A student can only have one preference per slot
        builder.HasIndex(x => new { x.CapstoneGroupId, x.ReviewSlotId, x.StudentMssv }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
