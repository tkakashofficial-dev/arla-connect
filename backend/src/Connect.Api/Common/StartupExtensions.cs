using Connect.Application.Common.Interfaces;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Connect.Api.Common;

public static class StartupExtensions
{
    /// <summary>Applies pending EF Core migrations and seeds data on startup.</summary>
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;

        try
        {
            var db = sp.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();

            var passwordHasher = sp.GetRequiredService<IPasswordHasher>();
            await DbSeeder.SeedAsync(db, passwordHasher);

            app.Logger.LogInformation("Database migrated and seeded successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex,
                "Database migration/seed failed. Is SQL Server running? (docker compose up -d)");
            throw;
        }
    }
}
