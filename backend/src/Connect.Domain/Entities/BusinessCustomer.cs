using Connect.Domain.Common;

namespace Connect.Domain.Entities;

/// <summary>
/// A B2B customer account (a company that buys from Arla).
/// Owns the login users and the orders placed under that account.
/// </summary>
public class BusinessCustomer : BaseEntity
{
    public string Name { get; set; } = null!;
    public string CustomerNumber { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
