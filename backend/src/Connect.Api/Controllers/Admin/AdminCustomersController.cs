using Connect.Application.Common;
using Connect.Application.Common.Models;
using Connect.Application.Features.Admin.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Connect.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/customers")]
[Authorize(Roles = Roles.PlatformAdmin)]
public class AdminCustomersController(IAdminCustomerService adminCustomerService) : ControllerBase
{
    /// <summary>All business customers with their order count and total spend.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminCustomerListDto>>> GetAll(
        [FromQuery] AdminCustomersQuery query, CancellationToken ct)
        => Ok(await adminCustomerService.GetAllAsync(query, ct));

    /// <summary>A single customer with its users and recent orders.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminCustomerDetailDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await adminCustomerService.GetByIdAsync(id, ct));
}
