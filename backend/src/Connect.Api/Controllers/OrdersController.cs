using Connect.Application.Common.Models;
using Connect.Application.Features.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // all order operations require an authenticated customer
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>Place a new order for the authenticated customer.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request, CancellationToken ct)
    {
        var order = await orderService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>List the authenticated customer's orders (most recent first).</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetMyOrders(
        [FromQuery] OrdersQuery query, CancellationToken ct)
        => Ok(await orderService.GetMyOrdersAsync(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await orderService.GetByIdAsync(id, ct));
}
