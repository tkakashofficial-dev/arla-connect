using Connect.Application.Common;
using Connect.Application.Features.Admin.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = Roles.PlatformAdmin)]
public class AdminDashboardController(IAdminDashboardService adminDashboardService) : ControllerBase
{
    /// <summary>Back-office summary metrics across all customers.</summary>
    [HttpGet]
    public async Task<ActionResult<AdminSummaryDto>> GetSummary(CancellationToken ct)
        => Ok(await adminDashboardService.GetSummaryAsync(ct));
}
