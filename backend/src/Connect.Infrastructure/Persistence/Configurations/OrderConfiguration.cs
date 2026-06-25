using Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Connect.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.OrderNumber).HasMaxLength(40).IsRequired();
        builder.Property(o => o.Currency).HasMaxLength(3).IsRequired();
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20); // readable in DB

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.BusinessCustomerId); // "my orders" queries

        // TotalAmount is derived from the lines in code, not a stored column.
        builder.Ignore(o => o.TotalAmount);

        // Map the encapsulated backing field instead of the read-only Lines property.
        builder.Metadata
            .FindNavigation(nameof(Order.Lines))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(o => o.Lines)
            .WithOne(l => l.Order)
            .HasForeignKey(l => l.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Claims)
            .WithOne(c => c.Order)
            .HasForeignKey(c => c.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
