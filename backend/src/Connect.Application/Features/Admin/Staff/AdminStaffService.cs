using Connect.Application.Common;
using Connect.Application.Common.Exceptions;
using Connect.Application.Common.Interfaces;
using Connect.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Features.Admin.Staff;

public class AdminStaffService(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    IValidator<CreateStaffRequest> validator) : IAdminStaffService
{
    private const string StaffCustomerNumber = "AC-STAFF-001";

    public async Task<IReadOnlyList<AdminStaffDto>> GetAllAsync(CancellationToken ct = default)
        => await db.Users.AsNoTracking()
            .Where(u => u.Role == Roles.PlatformAdmin)
            .OrderBy(u => u.FullName)
            .Select(u => new AdminStaffDto(u.Id, u.FullName, u.Email, u.Role, u.CreatedAtUtc))
            .ToListAsync(ct);

    public async Task<AdminStaffDto> CreateAsync(CreateStaffRequest request, CancellationToken ct = default)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            throw new ConflictException("An account with this email already exists.");

        var staffOrg = await db.BusinessCustomers.FirstOrDefaultAsync(c => c.CustomerNumber == StaffCustomerNumber, ct)
            ?? throw new NotFoundException("Staff organisation", StaffCustomerNumber);

        var user = new User
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = Roles.PlatformAdmin,
            BusinessCustomerId = staffOrg.Id
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return new AdminStaffDto(user.Id, user.FullName, user.Email, user.Role, user.CreatedAtUtc);
    }
}
