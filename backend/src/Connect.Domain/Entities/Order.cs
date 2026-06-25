using Connect.Domain.Common;
using Connect.Domain.Enums;

namespace Connect.Domain.Entities;

/// <summary>
/// Order aggregate root. Lines are only mutated through behaviour methods so the
/// order stays consistent (no orphan lines, totals always derived from lines).
/// </summary>
public class Order : BaseEntity
{
    private readonly List<OrderLine> _lines = new();

    private Order() { } // EF Core

    public Order(string orderNumber, Guid businessCustomerId, Guid placedByUserId, string currency = "DKK")
    {
        OrderNumber = orderNumber;
        BusinessCustomerId = businessCustomerId;
        PlacedByUserId = placedByUserId;
        Currency = currency;
        Status = OrderStatus.Pending;
    }

    public string OrderNumber { get; private set; } = null!;

    public Guid BusinessCustomerId { get; private set; }
    public BusinessCustomer BusinessCustomer { get; private set; } = null!;

    public Guid PlacedByUserId { get; private set; }

    public OrderStatus Status { get; private set; }
    public string Currency { get; private set; } = "DKK";

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
    public ICollection<Claim> Claims { get; private set; } = new List<Claim>();

    /// <summary>Derived from the lines — never stored, never out of sync.</summary>
    public decimal TotalAmount => _lines.Sum(l => l.LineTotal);

    public void AddLine(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        var existing = _lines.FirstOrDefault(l => l.ProductId == product.Id);
        if (existing is not null)
        {
            existing.Quantity += quantity;
            return;
        }

        _lines.Add(new OrderLine
        {
            ProductId = product.Id,
            UnitPrice = product.UnitPrice,
            Quantity = quantity
        });
    }

    public void UpdateStatus(OrderStatus status) => Status = status;
}
