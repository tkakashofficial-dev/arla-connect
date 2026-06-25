namespace Connect.Application.Features.Admin.Dashboard;

public interface IAdminDashboardService
{
    Task<AdminSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}
