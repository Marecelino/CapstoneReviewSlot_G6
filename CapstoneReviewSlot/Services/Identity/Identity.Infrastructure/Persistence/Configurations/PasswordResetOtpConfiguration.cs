using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class PasswordResetOtpConfiguration : IEntityTypeConfiguration<PasswordResetOtp>
{
    public void Configure(EntityTypeBuilder<PasswordResetOtp> builder)
    {
        builder.ToTable("PasswordResetOtps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.OtpHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.VerifiedAtUtc);

        builder.Property(x => x.UsedAtUtc);

        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.UserId);
    }
}