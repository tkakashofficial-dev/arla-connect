using Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Connect.Infrastructure.Persistence.Configurations;

public class BusinessCustomerConfiguration : IEntityTypeConfiguration<BusinessCustomer>
{
    public void Configure(EntityTypeBuilder<BusinessCustomer> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.CustomerNumber).HasMaxLength(40).IsRequired();
        builder.HasIndex(c => c.CustomerNumber).IsUnique();

        builder.HasMany(c => c.Users)
            .WithOne(u => u.BusinessCustomer)
            .HasForeignKey(u => u.BusinessCustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Orders)
            .WithOne(o => o.BusinessCustomer)
            .HasForeignKey(o => o.BusinessCustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
