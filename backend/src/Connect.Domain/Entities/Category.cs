using Connect.Domain.Common;

namespace Connect.Domain.Entities;

/// <summary>A product grouping, e.g. "Milk", "Cheese", "Butter".</summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
