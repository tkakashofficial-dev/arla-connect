using Connect.Application.Common.Models;

namespace Connect.Application.Features.Admin.Customers;

public interface IAdminCustomerService
{
    Task<PagedResult<AdminCustomerListDto>> GetAllAsync(AdminCustomersQuery query, CancellationToken ct = default);
    Task<AdminCustomerDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AdminCustomerListDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);
}
