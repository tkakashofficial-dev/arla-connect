using Connect.Application.Features.Admin.Products;
using Connect.Application.Features.Auth;
using Connect.Application.Features.Claims;
using Connect.Application.Features.Orders;
using Connect.Application.Features.Products;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Connect.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register every FluentValidation validator in this assembly.
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IClaimService, ClaimService>();

        // Admin (back-office)
        services.AddScoped<IProductAdminService, ProductAdminService>();

        return services;
    }
}
