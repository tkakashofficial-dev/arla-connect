using Connect.Domain.Enums;

namespace Connect.Application.Features.Admin.Claims;

public record AdminClaimDto(
    Guid Id,
    string ClaimNumber,
    string CustomerName,
    string OrderNumber,
    ClaimReason Reason,
    ClaimStatus Status,
    string Description,
    DateTime CreatedAtUtc);

public record UpdateClaimStatusRequest(ClaimStatus Status);
