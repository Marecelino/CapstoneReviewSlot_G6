using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Session.Domain.Entities;

namespace Session.Infrastructure.Persistence.Configurations;

public class CapstoneGroupMemberConfiguration : IEntityTypeConfiguration<CapstoneGroupMember>
{
    public void Configure(EntityTypeBuilder<CapstoneGroupMember> builder)
    {
        builder.ToTable("CapstoneGroupMember");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("MemberId");

        builder.Property(x => x.CapstoneGroupId).IsRequired();
        builder.Property(x => x.StudentMssv).IsRequired().HasMaxLength(50);
        builder.Property(x => x.StudentName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Department).IsRequired().HasMaxLength(20);

        builder.HasIndex(x => new { x.CapstoneGroupId, x.StudentMssv }).IsUnique();
        builder.HasIndex(x => x.StudentMssv);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
