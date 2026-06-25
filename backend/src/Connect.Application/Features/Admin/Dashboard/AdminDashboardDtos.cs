namespace Connect.Application.Features.Admin.Dashboard;

public record StatusCount(string Status, int Count);

public record AdminSummaryDto(
    int TotalOrders,
    decimal TotalRevenue,
    int OpenClaims,
    int ActiveProducts,
    int LowStockProducts,
    int TotalCustomers,
    IReadOnlyList<StatusCount> OrdersByStatus);
