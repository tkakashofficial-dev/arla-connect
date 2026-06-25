using Connect.Application.Common.Interfaces;
using Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Connect.Infrastructure.Persistence;

/// <summary>Seeds reference data and a demo login on first run (idempotent).</summary>
public static class DbSeeder
{
    public const string DemoEmail = "demo@arla-connect.test";
    public const string DemoPassword = "Password123!";

    public static async Task SeedAsync(AppDbContext db, IPasswordHasher passwordHasher, CancellationToken ct = default)
    {
        await SeedCatalogueAsync(db, ct);
        await SeedDemoAccountAsync(db, passwordHasher, ct);
    }

    private static async Task SeedCatalogueAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Categories.AnyAsync(ct))
            return;

        var milk = new Category { Name = "Milk", Slug = "milk" };
        var cheese = new Category { Name = "Cheese", Slug = "cheese" };
        var butter = new Category { Name = "Butter & Spreads", Slug = "butter-spreads" };
        var yogurt = new Category { Name = "Yogurt & Skyr", Slug = "yogurt-skyr" };

        var products = new List<Product>
        {
            new() { Sku = "MILK-001", Name = "Arla Økologisk Letmælk 1L", Description = "Organic semi-skimmed milk.", UnitPrice = 12.95m, StockQuantity = 500, Category = milk },
            new() { Sku = "MILK-002", Name = "Arla Sødmælk 1L", Description = "Whole milk.", UnitPrice = 11.50m, StockQuantity = 480, Category = milk },
            new() { Sku = "MILK-003", Name = "Cocio Chocolate Milk 250ml", Description = "Classic Danish chocolate milk.", UnitPrice = 9.50m, StockQuantity = 600, Category = milk },
            new() { Sku = "CHE-001", Name = "Arla Havarti 350g", Description = "Mild, creamy Danish cheese.", UnitPrice = 32.00m, StockQuantity = 220, Category = cheese },
            new() { Sku = "CHE-002", Name = "Arla Cheasy Skæreost 400g", Description = "Low-fat sliceable cheese.", UnitPrice = 28.50m, StockQuantity = 180, Category = cheese },
            new() { Sku = "CHE-003", Name = "Castello Blue 150g", Description = "Creamy Danish blue cheese.", UnitPrice = 35.00m, StockQuantity = 140, Category = cheese },
            new() { Sku = "BUT-001", Name = "Lurpak Smør 200g", Description = "Lightly salted butter.", UnitPrice = 25.00m, StockQuantity = 300, Category = butter },
            new() { Sku = "BUT-002", Name = "Arla Kærgården 200g", Description = "Butter blended with rapeseed oil.", UnitPrice = 22.50m, StockQuantity = 260, Category = butter },
            new() { Sku = "YOG-001", Name = "Arla Skyr Vanilje 450g", Description = "High-protein Icelandic-style skyr.", UnitPrice = 18.95m, StockQuantity = 340, Category = yogurt },
            new() { Sku = "YOG-002", Name = "Arla A38 Naturel 1L", Description = "Traditional Danish cultured milk.", UnitPrice = 16.50m, StockQuantity = 200, Category = yogurt }
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedDemoAccountAsync(AppDbContext db, IPasswordHasher passwordHasher, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(u => u.Email == DemoEmail, ct))
            return;

        var customer = new BusinessCustomer { Name = "Demo Foodservice A/S", CustomerNumber = "AC-DEMO-001" };
        var user = new User
        {
            Email = DemoEmail,
            FullName = "Demo Buyer",
            PasswordHash = passwordHasher.Hash(DemoPassword),
            Role = "Admin",
            BusinessCustomer = customer
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
    }
}
