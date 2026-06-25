using Connect.Application.Common.Models;
using Connect.Application.Features.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // browsing the catalogue does not require a login
public class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>Browse products with paging, search and category filtering.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetPaged(
        [FromQuery] ProductsQuery query, CancellationToken ct)
        => Ok(await productService.GetPagedAsync(query, ct));

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken ct)
        => Ok(await productService.GetCategoriesAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await productService.GetByIdAsync(id, ct));
}
