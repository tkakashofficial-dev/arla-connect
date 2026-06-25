using Connect.Application.Common.Models;

namespace Connect.Application.Features.Claims;

public interface IClaimService
{
    Task<ClaimDto> CreateAsync(CreateClaimRequest request, CancellationToken ct = default);
    Task<PagedResult<ClaimDto>> GetMyClaimsAsync(ClaimsQuery query, CancellationToken ct = default);
    Task<ClaimDto> GetByIdAsync(Guid id, CancellationToken ct = default);
}
