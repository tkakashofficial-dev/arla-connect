namespace Connect.Application.Features.Auth;

public record RegisterRequest(string Email, string Password, string FullName, string CompanyName);

public record LoginRequest(string Email, string Password);

public record AuthResponse(
    string Token,
    DateTime ExpiresAtUtc,
    string Email,
    string FullName,
    string Role,
    string CompanyName);
