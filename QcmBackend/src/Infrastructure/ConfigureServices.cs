using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Settings;
using QcmBackend.Infrastructure.Data;
using QcmBackend.Infrastructure.Data.Interceptors;
using QcmBackend.Infrastructure.Services;
using QcmBackend.Infrastructure.Identity;

namespace QcmBackend.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // 1. Configuration des settings
        services.Configure<AccountSettings>(configuration.GetSection("Account"));
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        services.Configure<GeneralSettings>(configuration.GetSection("General"));
        services.Configure<SecuritySettings>(configuration.GetSection("Security"));
        // ...autres settings...

        // 2. HttpClient
        services.AddHttpClient();

        // 3. DbContext + Interceptors
        if (!environment.IsEnvironment("Testing"))
        {
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(configuration.GetConnectionString("AppDb"));
            });
        }
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<AppDbContextInitializer>();

        // 4. Interceptors (audit, soft delete, events)
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        // 5. Services d’infrastructure
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IUser, CurrentUser>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ICookieService, CookieService>();
        // ...autres services...

        // 6. Identity
        services.AddIdentity<AppUser, IdentityRole<Guid>>(options => { /* options */ })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // 7. Hosted services, utilitaires, etc.
        // services.AddHostedService<...>();
        // services.AddSingleton<IHtmlRenderer, RazorHtmlRenderer>();

        return services;
    }
}
