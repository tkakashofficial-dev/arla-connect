using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Application.Common.Models;
using Connect.Application.Features.Orders;
using Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Admin.Orders;

public class AdminOrderService(IAppDbContext db) : IAdminOrderService
{
    public async Task<PagedResult<AdminOrderDto>> GetAllAsync(OrdersQuery query, CancellationToken ct = default)
    {
        // Admin sees every customer's orders (not scoped).
        var orders = db.Orders.AsNoTracking();
        var totalCount = await orders.CountAsync(ct);

        var items = await orders
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .Select(o => new AdminOrderDto(
                o.Id, o.OrderNumber, o.BusinessCustomer.Name, o.Status, o.Currency,
                o.Lines.Sum(l => l.UnitPrice * l.Quantity), o.CreatedAtUtc))
            .ToListAsync(ct);

        return new PagedResult<AdminOrderDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new NotFoundException(nameof(Order), id);

        order.UpdateStatus(request.Status);
        await db.SaveChangesAsync(ct);
    }
}
