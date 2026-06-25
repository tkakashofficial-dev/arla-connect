using Connect.Application.Common.Models;

namespace Connect.Application.Features.Products;

public record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal UnitPrice,
    string Currency,
    int StockQuantity,
    string? ImageUrl,
    Guid CategoryId,
    string CategoryName,
    bool IsActive);

public record CategoryDto(Guid Id, string Name, string Slug);

/// <summary>Query parameters for browsing the catalogue.</summary>
public class ProductsQuery : PagedRequest
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
}
