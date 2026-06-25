using Connect.Domain.Enums;

namespace Connect.Application.Features.Admin.Orders;

public record AdminOrderDto(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    OrderStatus Status,
    string Currency,
    decimal TotalAmount,
    DateTime CreatedAtUtc);

public record UpdateOrderStatusRequest(OrderStatus Status);
