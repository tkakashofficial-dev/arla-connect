using Connect.Application.Common;
using Connect.Application.Common.Models;
using Connect.Application.Features.Admin.Claims;
using Connect.Application.Features.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/claims")]
[Authorize(Roles = Roles.PlatformAdmin)]
public class AdminClaimsController(IAdminClaimService adminClaimService) : ControllerBase
{
    /// <summary>All claims across every customer.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminClaimDto>>> GetAll(
        [FromQuery] ClaimsQuery query, CancellationToken ct)
        => Ok(await adminClaimService.GetAllAsync(query, ct));

    /// <summary>Move a claim to a new status (e.g. UnderReview, Approved, Rejected, Resolved).</summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateClaimStatusRequest request, CancellationToken ct)
    {
        await adminClaimService.UpdateStatusAsync(id, request, ct);
        return NoContent();
    }
}
