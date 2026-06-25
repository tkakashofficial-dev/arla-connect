using Connect.Application.Common.Models;
using Connect.Application.Features.Claims;

namespace Connect.Application.Features.Admin.Claims;

public interface IAdminClaimService
{
    Task<PagedResult<AdminClaimDto>> GetAllAsync(ClaimsQuery query, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, UpdateClaimStatusRequest request, CancellationToken ct = default);
}
