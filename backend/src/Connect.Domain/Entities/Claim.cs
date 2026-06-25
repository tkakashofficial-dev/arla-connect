using Connect.Domain.Common;
using Connect.Domain.Enums;

namespace Connect.Domain.Entities;

/// <summary>A claim raised by a customer against a specific order.</summary>
public class Claim : BaseEntity
{
    private Claim() { } // EF Core

    public Claim(string claimNumber, Guid orderId, ClaimReason reason, string description)
    {
        ClaimNumber = claimNumber;
        OrderId = orderId;
        Reason = reason;
        Description = description;
        Status = ClaimStatus.Open;
    }

    public string ClaimNumber { get; private set; } = null!;

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public ClaimReason Reason { get; private set; }
    public string Description { get; private set; } = null!;
    public ClaimStatus Status { get; private set; }

    public void UpdateStatus(ClaimStatus status) => Status = status;
}
