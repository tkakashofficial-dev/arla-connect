using Connect.Application.Common.Models;
using Connect.Domain.Enums;

namespace Connect.Application.Features.Orders;

public record CreateOrderLineRequest(Guid ProductId, int Quantity);

public record CreateOrderRequest(IReadOnlyList<CreateOrderLineRequest> Lines);

public record OrderLineDto(
    Guid ProductId,
    string Sku,
    string ProductName,
    string? ImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public record OrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    string Currency,
    decimal TotalAmount,
    DateTime CreatedAtUtc,
    IReadOnlyList<OrderLineDto> Lines);

public class OrdersQuery : PagedRequest;
