using Connect.Domain.Common;

namespace Connect.Domain.Entities;

/// <summary>An Arla product a business customer can browse and order.</summary>
public class Product : BaseEntity
{
    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "DKK"; // Danish krone

    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
