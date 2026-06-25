namespace Connect.Application.Features.Admin.Products;

public record CreateProductRequest(
    string Sku,
    string Name,
    string? Description,
    decimal UnitPrice,
    string Currency,
    int StockQuantity,
    string? ImageUrl,
    Guid CategoryId);

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal UnitPrice,
    string Currency,
    int StockQuantity,
    string? ImageUrl,
    Guid CategoryId,
    bool IsActive);
