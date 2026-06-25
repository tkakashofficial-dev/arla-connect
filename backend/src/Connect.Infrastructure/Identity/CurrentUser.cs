using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Connect.Application.Common;
using Connect.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Connect.Infrastructure.Identity;

/// <summary>Reads the authenticated user out of the current request's JWT claims.</summary>
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    // Read both the raw ("sub") and framework-mapped (NameIdentifier) claim names
    // so this works regardless of the inbound-claim-mapping setting.
    public Guid? UserId => ParseGuid(
        Find(ClaimTypes.NameIdentifier) ?? Find(JwtRegisteredClaimNames.Sub));

    public Guid? BusinessCustomerId => ParseGuid(Find(AppClaimTypes.BusinessCustomerId));

    public string? Email => Find(ClaimTypes.Email) ?? Find(JwtRegisteredClaimNames.Email);

    private string? Find(string claimType) => Principal?.FindFirst(claimType)?.Value;

    private static Guid? ParseGuid(string? value)
        => Guid.TryParse(value, out var guid) ? guid : null;
}
