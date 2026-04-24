using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class LecturerConfiguration : IEntityTypeConfiguration<Lecturer>
{
    public void Configure(EntityTypeBuilder<Lecturer> builder)
    {
        builder.ToTable("Lecturer");

        builder.HasKey(l => l.LecturerId);

        builder.Property(l => l.LecturerId)
            .ValueGeneratedOnAdd();

        builder.Property(l => l.UserId)
            .IsRequired();

        builder.HasIndex(l => l.UserId)
            .IsUnique();                         // 1 user chỉ có 1 Lecturer record

        builder.Property(l => l.LecturerCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(l => l.LecturerCode)
            .IsUnique();

        builder.Property(l => l.Department)
            .HasMaxLength(100);

        // Quan hệ 1-1: Lecturer.UserId → User.Id (Guid PK, mapped ra cột UserId)
        builder.HasOne(l => l.User)
               .WithOne(u => u.Lecturer)
               .HasForeignKey<Lecturer>(l => l.UserId)
               .HasPrincipalKey<User>(u => u.Id)
               .OnDelete(DeleteBehavior.Cascade);

        // Fix Warning 10622: Add matching query filter for Lecturer since User has a query filter
        builder.HasQueryFilter(l => !l.User.IsDeleted);
    }
}
