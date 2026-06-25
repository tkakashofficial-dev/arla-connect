using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Application.Common.Models;
using Connect.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Claims;

public class ClaimService(
    IAppDbContext db,
    ICurrentUser currentUser,
    IValidator<CreateClaimRequest> validator) : IClaimService
{
    public async Task<ClaimDto> CreateAsync(CreateClaimRequest request, CancellationToken ct = default)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var customerId = currentUser.BusinessCustomerId
            ?? throw new UnauthorizedAccessException("No authenticated customer.");

        // The order must exist AND belong to the caller's customer.
        var order = await db.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.BusinessCustomerId == customerId, ct)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var claim = new Claim(GenerateClaimNumber(), order.Id, request.Reason, request.Description.Trim());
        db.Claims.Add(claim);
        await db.SaveChangesAsync(ct);

        return new ClaimDto(claim.Id, claim.ClaimNumber, order.Id, order.OrderNumber,
            claim.Reason, claim.Status, claim.Description, claim.CreatedAtUtc);
    }

    public async Task<PagedResult<ClaimDto>> GetMyClaimsAsync(ClaimsQuery query, CancellationToken ct = default)
    {
        var customerId = currentUser.BusinessCustomerId
            ?? throw new UnauthorizedAccessException("No authenticated customer.");

        var claims = db.Claims.AsNoTracking()
            .Where(c => c.Order.BusinessCustomerId == customerId);

        var totalCount = await claims.CountAsync(ct);

        var items = await claims
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .Select(c => new ClaimDto(c.Id, c.ClaimNumber, c.OrderId, c.Order.OrderNumber,
                c.Reason, c.Status, c.Description, c.CreatedAtUtc))
            .ToListAsync(ct);

        return new PagedResult<ClaimDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<ClaimDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var customerId = currentUser.BusinessCustomerId
            ?? throw new UnauthorizedAccessException("No authenticated customer.");

        var claim = await db.Claims.AsNoTracking()
            .Where(c => c.Id == id && c.Order.BusinessCustomerId == customerId)
            .Select(c => new ClaimDto(c.Id, c.ClaimNumber, c.OrderId, c.Order.OrderNumber,
                c.Reason, c.Status, c.Description, c.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);

        return claim ?? throw new NotFoundException(nameof(Claim), id);
    }

    private static string GenerateClaimNumber()
        => $"CLM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
}
