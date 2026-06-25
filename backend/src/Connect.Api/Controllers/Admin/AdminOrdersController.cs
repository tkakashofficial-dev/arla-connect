using Connect.Application.Common;
using Connect.Application.Common.Models;
using Connect.Application.Features.Admin.Orders;
using Connect.Application.Features.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = Roles.PlatformAdmin)]
public class AdminOrdersController(IAdminOrderService adminOrderService) : ControllerBase
{
    /// <summary>All orders across every customer.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminOrderDto>>> GetAll(
        [FromQuery] OrdersQuery query, CancellationToken ct)
        => Ok(await adminOrderService.GetAllAsync(query, ct));

    /// <summary>Move an order to a new status (e.g. Confirmed, Shipped, Delivered).</summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateOrderStatusRequest request, CancellationToken ct)
    {
        await adminOrderService.UpdateStatusAsync(id, request, ct);
        return NoContent();
    }
}
