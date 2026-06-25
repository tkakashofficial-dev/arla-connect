using Connect.Domain.Common;

namespace Connect.Domain.Entities;

/// <summary>A person who logs in on behalf of a <see cref="BusinessCustomer"/>.</summary>
public class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;

    /// <summary>Application role, e.g. "Buyer" or "Admin".</summary>
    public string Role { get; set; } = "Buyer";

    public Guid BusinessCustomerId { get; set; }
    public BusinessCustomer BusinessCustomer { get; set; } = null!;
}
