using Connect.Application.Common.Models;
using Connect.Domain.Enums;

namespace Connect.Application.Features.Claims;

public record CreateClaimRequest(Guid OrderId, ClaimReason Reason, string Description);

public record ClaimDto(
    Guid Id,
    string ClaimNumber,
    Guid OrderId,
    string OrderNumber,
    ClaimReason Reason,
    ClaimStatus Status,
    string Description,
    DateTime CreatedAtUtc);

public class ClaimsQuery : PagedRequest;
