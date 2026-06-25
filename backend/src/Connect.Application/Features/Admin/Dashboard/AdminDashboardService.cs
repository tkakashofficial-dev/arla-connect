using Connect.Application.Common.Interfaces;
using Connect.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Admin.Dashboard;

public class AdminDashboardService(IAppDbContext db) : IAdminDashboardService
{
    private const int LowStockThreshold = 50;
    private const string StaffCustomerNumber = "AC-STAFF-001";

    public async Task<AdminSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var totalOrders = await db.Orders.CountAsync(ct);

        var totalRevenue = await db.OrderLines.SumAsync(l => (decimal?)(l.UnitPrice * l.Quantity), ct) ?? 0m;

        var openClaims = await db.Claims.CountAsync(
            c => c.Status == ClaimStatus.Open || c.Status == ClaimStatus.UnderReview, ct);

        var activeProducts = await db.Products.CountAsync(p => p.IsActive, ct);
        var lowStock = await db.Products.CountAsync(p => p.IsActive && p.StockQuantity < LowStockThreshold, ct);
        var totalCustomers = await db.BusinessCustomers.CountAsync(c => c.CustomerNumber != StaffCustomerNumber, ct);

        // Group in SQL, then format the enum name in memory (translatable + safe).
        var grouped = await db.Orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);
        var byStatus = grouped.Select(g => new StatusCount(g.Status.ToString(), g.Count)).ToList();

        return new AdminSummaryDto(totalOrders, totalRevenue, openClaims, activeProducts, lowStock, totalCustomers, byStatus);
    }
}
