using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Application.Common.Models;
using Connect.Application.Features.Claims;
using Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Admin.Claims;

public class AdminClaimService(IAppDbContext db) : IAdminClaimService
{
    public async Task<PagedResult<AdminClaimDto>> GetAllAsync(ClaimsQuery query, CancellationToken ct = default)
    {
        var claims = db.Claims.AsNoTracking();
        var totalCount = await claims.CountAsync(ct);

        var items = await claims
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .Select(c => new AdminClaimDto(
                c.Id, c.ClaimNumber, c.Order.BusinessCustomer.Name, c.Order.OrderNumber,
                c.Reason, c.Status, c.Description, c.CreatedAtUtc))
            .ToListAsync(ct);

        return new PagedResult<AdminClaimDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task UpdateStatusAsync(Guid id, UpdateClaimStatusRequest request, CancellationToken ct = default)
    {
        var claim = await db.Claims.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException(nameof(Claim), id);

        claim.UpdateStatus(request.Status);
        await db.SaveChangesAsync(ct);
    }
}
