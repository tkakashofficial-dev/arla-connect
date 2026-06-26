using Connect.Application.Common.Models;
using Connect.Domain.Enums;

namespace Connect.Application.Features.Admin.Customers;

public record AdminCustomerListDto(
    Guid Id,
    string Name,
    string CustomerNumber,
    int UserCount,
    int OrderCount,
    decimal TotalSpend,
    DateTime CreatedAtUtc);

public record AdminCustomerUserDto(Guid Id, string FullName, string Email, string Role);

public record AdminCustomerOrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAtUtc);

public record AdminCustomerDetailDto(
    Guid Id,
    string Name,
    string CustomerNumber,
    DateTime CreatedAtUtc,
    decimal TotalSpend,
    int OrderCount,
    IReadOnlyList<AdminCustomerUserDto> Users,
    IReadOnlyList<AdminCustomerOrderDto> RecentOrders);

public class AdminCustomersQuery : PagedRequest
{
    public string? Search { get; set; }
}

public record CreateCustomerRequest(string Name);
