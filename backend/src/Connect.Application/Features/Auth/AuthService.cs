using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Auth;

public class AuthService(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwt,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        await registerValidator.ValidateAndThrowAsync(request, ct);

        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            throw new ConflictException("An account with this email already exists.");

        var customer = new BusinessCustomer
        {
            Name = request.CompanyName.Trim(),
            CustomerNumber = GenerateCustomerNumber()
        };

        var user = new User
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = "Admin", // first user of a company account is its admin
            BusinessCustomer = customer
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return BuildResponse(user, customer.Name);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        await loginValidator.ValidateAndThrowAsync(request, ct);

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users
            .Include(u => u.BusinessCustomer)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        // Same error whether the email or the password is wrong (no account enumeration).
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildResponse(user, user.BusinessCustomer.Name);
    }

    private AuthResponse BuildResponse(User user, string companyName)
    {
        var (token, expiresAt) = jwt.Generate(user);
        return new AuthResponse(token, expiresAt, user.Email, user.FullName, user.Role, companyName);
    }

    private static string GenerateCustomerNumber()
        => $"AC-{DateTime.UtcNow:yyMM}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
}
