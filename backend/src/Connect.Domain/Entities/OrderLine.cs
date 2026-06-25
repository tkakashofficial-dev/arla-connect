using Connect.Domain.Common;

namespace Connect.Domain.Entities;

/// <summary>A single product line within an <see cref="Order"/>.</summary>
public class OrderLine : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    /// <summary>Price captured at time of order (products may reprice later).</summary>
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}
