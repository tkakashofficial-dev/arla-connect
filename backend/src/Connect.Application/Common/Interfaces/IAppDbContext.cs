using Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the database the Application layer depends on.
/// Keeps Application free of a hard reference to the concrete EF Core context,
/// which makes the CQRS handlers easy to unit-test.
/// </summary>
public interface IAppDbContext
{
    DbSet<BusinessCustomer> BusinessCustomers { get; }
    DbSet<User> Users { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderLine> OrderLines { get; }
    DbSet<Claim> Claims { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
