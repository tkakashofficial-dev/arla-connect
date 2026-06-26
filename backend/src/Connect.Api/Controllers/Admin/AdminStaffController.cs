using Connect.Application.Common;
using Connect.Application.Features.Admin.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/staff")]
[Authorize(Roles = Roles.PlatformAdmin)]
public class AdminStaffController(IAdminStaffService adminStaffService) : ControllerBase
{
    /// <summary>All Arla staff (platform admins).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AdminStaffDto>>> GetAll(CancellationToken ct)
        => Ok(await adminStaffService.GetAllAsync(ct));

    /// <summary>Create a new staff member (platform admin).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AdminStaffDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AdminStaffDto>> Create(CreateStaffRequest request, CancellationToken ct)
    {
        var staff = await adminStaffService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), new { id = staff.Id }, staff);
    }
}
