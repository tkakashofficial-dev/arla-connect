namespace Connect.Application.Features.Admin.Staff;

public interface IAdminStaffService
{
    Task<IReadOnlyList<AdminStaffDto>> GetAllAsync(CancellationToken ct = default);
    Task<AdminStaffDto> CreateAsync(CreateStaffRequest request, CancellationToken ct = default);
}
