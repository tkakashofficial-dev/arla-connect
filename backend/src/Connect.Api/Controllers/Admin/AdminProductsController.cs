using Connect.Application.Common;
using Connect.Application.Common.Models;
using Connect.Application.Features.Admin.Products;
using Connect.Application.Features.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = Roles.PlatformAdmin)] // Arla staff only — enforced server-side
public class AdminProductsController(IProductAdminService productAdminService) : ControllerBase
{
    /// <summary>List every product (including inactive) for management.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
        [FromQuery] ProductsQuery query, CancellationToken ct)
        => Ok(await productAdminService.GetAllAsync(query, ct));

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken ct)
    {
        var product = await productAdminService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, UpdateProductRequest request, CancellationToken ct)
        => Ok(await productAdminService.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await productAdminService.DeleteAsync(id, ct);
        return NoContent();
    }
}
