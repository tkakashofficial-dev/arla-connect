using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Application.Common.Models;
using Connect.Application.Features.Products;
using Connect.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Admin.Products;

public class ProductAdminService(
    IAppDbContext db,
    IValidator<CreateProductRequest> createValidator,
    IValidator<UpdateProductRequest> updateValidator) : IProductAdminService
{
    public async Task<PagedResult<ProductDto>> GetAllAsync(ProductsQuery query, CancellationToken ct = default)
    {
        // Admin sees everything, including inactive (soft-deleted) products.
        var products = db.Products.AsNoTracking();

        if (query.CategoryId is { } categoryId)
            products = products.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            products = products.Where(p => p.Name.Contains(term) || p.Sku.Contains(term));
        }

        var totalCount = await products.CountAsync(ct);

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

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);

        var category = await GetCategoryAsync(request.CategoryId, ct);

        if (await db.Products.AnyAsync(p => p.Sku == request.Sku, ct))
            throw new ConflictException($"A product with SKU \"{request.Sku}\" already exists.");

        var product = new Product
        {
            Sku = request.Sku.Trim(),
            Name = request.Name.Trim(),
            Description = request.Description,
            UnitPrice = request.UnitPrice,
            Currency = request.Currency.ToUpperInvariant(),
            StockQuantity = request.StockQuantity,
            ImageUrl = request.ImageUrl,
            CategoryId = category.Id,
            IsActive = true
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);

        return Map(product, category.Name);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        await updateValidator.ValidateAndThrowAsync(request, ct);

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException(nameof(Product), id);

        var category = await GetCategoryAsync(request.CategoryId, ct);

        product.Name = request.Name.Trim();
        product.Description = request.Description;
        product.UnitPrice = request.UnitPrice;
        product.Currency = request.Currency.ToUpperInvariant();
        product.StockQuantity = request.StockQuantity;
        product.ImageUrl = request.ImageUrl;
        product.CategoryId = category.Id;
        product.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);

        return Map(product, category.Name);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException(nameof(Product), id);

        // Soft delete: products may be referenced by historical orders.
        product.IsActive = false;
        await db.SaveChangesAsync(ct);
    }

    private async Task<Category> GetCategoryAsync(Guid categoryId, CancellationToken ct)
        => await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId, ct)
            ?? throw new NotFoundException(nameof(Category), categoryId);

    private static ProductDto Map(Product p, string categoryName) => new(
        p.Id, p.Sku, p.Name, p.Description, p.UnitPrice, p.Currency,
        p.StockQuantity, p.ImageUrl, p.CategoryId, categoryName, p.IsActive);
}
