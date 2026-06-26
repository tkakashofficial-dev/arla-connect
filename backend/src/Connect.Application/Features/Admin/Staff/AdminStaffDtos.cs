namespace Connect.Application.Features.Admin.Staff;

public record AdminStaffDto(Guid Id, string FullName, string Email, string Role, DateTime CreatedAtUtc);

public record CreateStaffRequest(string FullName, string Email, string Password);
