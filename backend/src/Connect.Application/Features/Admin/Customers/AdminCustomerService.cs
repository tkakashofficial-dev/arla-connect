using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Application.Common.Models;
using Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Admin.Customers;

public class AdminCustomerService(IAppDbContext db) : IAdminCustomerService
{
    private const string StaffCustomerNumber = "AC-STAFF-001";

    public async Task<PagedResult<AdminCustomerListDto>> GetAllAsync(AdminCustomersQuery query, CancellationToken ct = default)
    {
        var customers = db.BusinessCustomers.AsNoTracking()
            .Where(c => c.CustomerNumber != StaffCustomerNumber);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            customers = customers.Where(c => c.Name.Contains(term) || c.CustomerNumber.Contains(term));
        }

        var totalCount = await customers.CountAsync(ct);

        var items = await customers
            .OrderBy(c => c.Name)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .Select(c => new AdminCustomerListDto(
                c.Id,
                c.Name,
                c.CustomerNumber,
                c.Users.Count,
                c.Orders.Count,
                c.Orders.SelectMany(o => o.Lines).Sum(l => l.UnitPrice * l.Quantity),
                c.CreatedAtUtc))
            .ToListAsync(ct);

        return new PagedResult<AdminCustomerListDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<AdminCustomerDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await db.BusinessCustomers.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new AdminCustomerDetailDto(
                c.Id,
                c.Name,
                c.CustomerNumber,
                c.CreatedAtUtc,
                c.Orders.SelectMany(o => o.Lines).Sum(l => l.UnitPrice * l.Quantity),
                c.Orders.Count,
                c.Users
                    .OrderBy(u => u.FullName)
                    .Select(u => new AdminCustomerUserDto(u.Id, u.FullName, u.Email, u.Role))
                    .ToList(),
                c.Orders
                    .OrderByDescending(o => o.CreatedAtUtc)
                    .Take(10)
                    .Select(o => new AdminCustomerOrderDto(
                        o.Id, o.OrderNumber, o.Status,
                        o.Lines.Sum(l => l.UnitPrice * l.Quantity), o.CreatedAtUtc))
                    .ToList()))
            .FirstOrDefaultAsync(ct);

        return customer ?? throw new NotFoundException(nameof(BusinessCustomer), id);
    }
}
