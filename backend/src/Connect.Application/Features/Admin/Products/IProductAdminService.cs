using Connect.Application.Common.Models;
using Connect.Application.Features.Products;

namespace Connect.Application.Features.Admin.Products;

public interface IProductAdminService
{
    Task<PagedResult<ProductDto>> GetAllAsync(ProductsQuery query, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
