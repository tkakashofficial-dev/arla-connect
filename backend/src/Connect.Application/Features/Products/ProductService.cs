using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Products;

public class ProductService(IAppDbContext db) : IProductService
{
    public async Task<PagedResult<ProductDto>> GetPagedAsync(ProductsQuery query, CancellationToken ct = default)
    {
        // AsNoTracking: read-only, so skip change-tracking overhead.
        var products = db.Products.AsNoTracking().Where(p => p.IsActive);

        if (query.CategoryId is { } categoryId)
            products = products.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            products = products.Where(p => p.Name.Contains(term) || p.Sku.Contains(term));
        }

        var totalCount = await products.CountAsync(ct);

        // Project straight to the DTO so SQL only selects the columns we need.
        var items = await products
            .OrderBy(p => p.Name)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .Select(p => new ProductDto(
                p.Id, p.Sku, p.Name, p.Description, p.UnitPrice, p.Currency,
                p.StockQuantity, p.ImageUrl, p.CategoryId, p.Category.Name, p.IsActive))
            .ToListAsync(ct);

        return new PagedResult<ProductDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await db.Products.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto(
                p.Id, p.Sku, p.Name, p.Description, p.UnitPrice, p.Currency,
                p.StockQuantity, p.ImageUrl, p.CategoryId, p.Category.Name, p.IsActive))
            .FirstOrDefaultAsync(ct);

        return product ?? throw new NotFoundException(nameof(ProductDto), id);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
        => await db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug))
            .ToListAsync(ct);
}
