using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Application.Common.Models;
using Connect.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Orders;

public class OrderService(
    IAppDbContext db,
    ICurrentUser currentUser,
    IValidator<CreateOrderRequest> validator) : IOrderService
{
    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var customerId = currentUser.BusinessCustomerId
            ?? throw new UnauthorizedAccessException("No authenticated customer.");
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        // Load every requested product once (tracked, because we adjust stock).
        var productIds = request.Lines.Select(l => l.ProductId).Distinct().ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToDictionaryAsync(p => p.Id, ct);

        var order = new Order(GenerateOrderNumber(), customerId, userId);

        foreach (var line in request.Lines)
        {
            if (!products.TryGetValue(line.ProductId, out var product))
                throw new NotFoundException(nameof(Product), line.ProductId);

            if (product.StockQuantity < line.Quantity)
                throw new ConflictException($"Insufficient stock for \"{product.Name}\".");

            product.StockQuantity -= line.Quantity;
            order.AddLine(product, line.Quantity);
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);

        return MapToDto(order, products);
    }

    public async Task<PagedResult<OrderDto>> GetMyOrdersAsync(OrdersQuery query, CancellationToken ct = default)
    {
        var customerId = currentUser.BusinessCustomerId
            ?? throw new UnauthorizedAccessException("No authenticated customer.");

        // Scoped to the caller's customer — a customer never sees another's orders.
        var orders = db.Orders.AsNoTracking()
            .Where(o => o.BusinessCustomerId == customerId);

        var totalCount = await orders.CountAsync(ct);

        var items = await orders
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .Select(o => ProjectToDto(o))
            .ToListAsync(ct);

        return new PagedResult<OrderDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var customerId = currentUser.BusinessCustomerId
            ?? throw new UnauthorizedAccessException("No authenticated customer.");

        var order = await db.Orders.AsNoTracking()
            .Where(o => o.Id == id && o.BusinessCustomerId == customerId)
            .Select(o => ProjectToDto(o))
            .FirstOrDefaultAsync(ct);

        return order ?? throw new NotFoundException(nameof(Order), id);
    }

    // EF-translatable projection used by the read queries.
    private static OrderDto ProjectToDto(Order o) => new(
        o.Id,
        o.OrderNumber,
        o.Status,
        o.Currency,
        o.Lines.Sum(l => l.UnitPrice * l.Quantity),
        o.CreatedAtUtc,
        o.Lines.Select(l => new OrderLineDto(
            l.ProductId, l.Product.Sku, l.Product.Name, l.Quantity, l.UnitPrice, l.UnitPrice * l.Quantity))
            .ToList());

    // In-memory mapping right after creation (uses the products we already loaded).
    private static OrderDto MapToDto(Order order, IReadOnlyDictionary<Guid, Product> products) => new(
        order.Id,
        order.OrderNumber,
        order.Status,
        order.Currency,
        order.TotalAmount,
        order.CreatedAtUtc,
        order.Lines.Select(l =>
        {
            var p = products[l.ProductId];
            return new OrderLineDto(l.ProductId, p.Sku, p.Name, l.Quantity, l.UnitPrice, l.LineTotal);
        }).ToList());

    private static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
}
