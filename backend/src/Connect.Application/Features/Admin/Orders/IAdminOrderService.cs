using Connect.Application.Common.Models;
using Connect.Application.Features.Orders;

namespace Connect.Application.Features.Admin.Orders;

public interface IAdminOrderService
{
    Task<PagedResult<AdminOrderDto>> GetAllAsync(OrdersQuery query, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken ct = default);
}
