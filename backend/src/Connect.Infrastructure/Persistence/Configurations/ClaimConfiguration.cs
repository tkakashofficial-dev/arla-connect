using Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Connect.Infrastructure.Persistence.Configurations;

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.Property(c => c.ClaimNumber).HasMaxLength(40).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(2000).IsRequired();
        builder.Property(c => c.Reason).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(c => c.ClaimNumber).IsUnique();
        builder.HasIndex(c => c.OrderId);
    }
}
