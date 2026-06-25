using Connect.Application.Common.Models;

namespace Connect.Application.Features.Products;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetPagedAsync(ProductsQuery query, CancellationToken ct = default);
    Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
}
