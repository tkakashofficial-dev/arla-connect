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

    /// <summary>Upload a product image; returns the URL to store on the product.</summary>
    [HttpPost("image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<object>> UploadImage(
        IFormFile file, [FromServices] IWebHostEnvironment env, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded.");
        if (file.Length > 4 * 1024 * 1024)
            return BadRequest("File too large (max 4 MB).");

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
            return BadRequest("Unsupported file type. Use JPG, PNG or WEBP.");

        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        var uploads = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(uploads);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploads, fileName);
        await using (var stream = System.IO.File.Create(fullPath))
            await file.CopyToAsync(stream, ct);

        var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        return Ok(new { url });
    }
}
