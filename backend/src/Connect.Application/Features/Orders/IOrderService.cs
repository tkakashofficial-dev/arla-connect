using Connect.Application.Common.Models;

namespace Connect.Application.Features.Orders;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task<PagedResult<OrderDto>> GetMyOrdersAsync(OrdersQuery query, CancellationToken ct = default);
    Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default);
}
