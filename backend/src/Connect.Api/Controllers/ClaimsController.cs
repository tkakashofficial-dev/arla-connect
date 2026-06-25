using Connect.Application.Common.Models;
using Connect.Application.Features.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClaimsController(IClaimService claimService) : ControllerBase
{
    /// <summary>Raise a claim against one of the customer's orders.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClaimDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ClaimDto>> Create(CreateClaimRequest request, CancellationToken ct)
    {
        var claim = await claimService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = claim.Id }, claim);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ClaimDto>>> GetMyClaims(
        [FromQuery] ClaimsQuery query, CancellationToken ct)
        => Ok(await claimService.GetMyClaimsAsync(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClaimDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await claimService.GetByIdAsync(id, ct));
}
