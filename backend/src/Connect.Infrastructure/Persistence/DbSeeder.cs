using Connect.Application.Common.Interfaces;
using Connect.Domain.Entities;
using Connect.Domain.Enums;
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
        await SeedDemoOrderHistoryAsync(db, ct);
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

    /// <summary>Backdated orders + claims for the demo account so the dashboard charts look real.</summary>
    private static async Task SeedDemoOrderHistoryAsync(AppDbContext db, CancellationToken ct)
    {
        var customer = await db.BusinessCustomers
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.CustomerNumber == "AC-DEMO-001", ct);
        if (customer is null || customer.Users.Count == 0)
            return;

        if (await db.Orders.AnyAsync(o => o.BusinessCustomerId == customer.Id, ct))
            return;

        var userId = customer.Users.First().Id;
        var products = await db.Products.ToDictionaryAsync(p => p.Sku, ct);

        var specs = new (int Days, OrderStatus Status, (string Sku, int Qty)[] Items)[]
        {
            (150, OrderStatus.Delivered, [("MILK-001", 12), ("CHE-001", 4)]),
            (120, OrderStatus.Delivered, [("BUT-001", 6), ("YOG-001", 8)]),
            (95, OrderStatus.Delivered, [("CHE-003", 3), ("MILK-002", 14)]),
            (64, OrderStatus.Delivered, [("YOG-002", 5), ("BUT-002", 4), ("MILK-003", 10)]),
            (38, OrderStatus.Shipped, [("CHE-002", 6), ("MILK-001", 9)]),
            (16, OrderStatus.Confirmed, [("YOG-001", 7), ("CHE-001", 5)]),
            (4, OrderStatus.Pending, [("MILK-002", 10), ("BUT-001", 3)]),
        };

        var seq = 4096;
        foreach (var spec in specs)
        {
            var createdAt = DateTime.UtcNow.AddDays(-spec.Days);
            var order = new Order($"ORD-{createdAt:yyyyMMdd}-{seq:X4}", customer.Id, userId);
            foreach (var (sku, qty) in spec.Items)
                if (products.TryGetValue(sku, out var product))
                    order.AddLine(product, qty);
            order.UpdateStatus(spec.Status);
            order.CreatedAtUtc = createdAt; // backdated; honoured by the DbContext
            db.Orders.Add(order);
            seq++;
        }
        await db.SaveChangesAsync(ct);

        // A couple of claims against the oldest delivered orders.
        var delivered = await db.Orders
            .Where(o => o.BusinessCustomerId == customer.Id && o.Status == OrderStatus.Delivered)
            .OrderBy(o => o.CreatedAtUtc)
            .Take(2)
            .ToListAsync(ct);

        if (delivered.Count == 2)
        {
            var c1 = new Claim($"CLM-{delivered[0].CreatedAtUtc:yyyyMMdd}-A1B2", delivered[0].Id,
                ClaimReason.DamagedGoods, "Two cartons arrived leaking on delivery.");
            c1.UpdateStatus(ClaimStatus.Resolved);
            c1.CreatedAtUtc = delivered[0].CreatedAtUtc.AddDays(2);

            var c2 = new Claim($"CLM-{delivered[1].CreatedAtUtc:yyyyMMdd}-C3D4", delivered[1].Id,
                ClaimReason.WrongItem, "Received Havarti instead of the blue cheese ordered.");
            c2.CreatedAtUtc = delivered[1].CreatedAtUtc.AddDays(1);

            db.Claims.AddRange(c1, c2);
            await db.SaveChangesAsync(ct);
        }
    }
}
