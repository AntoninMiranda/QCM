using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Application.Common.Settings;
using QcmBackend.Infrastructure.Data;
using QcmBackend.Infrastructure.Data.Interceptors;
using QcmBackend.Infrastructure.Identity;
using QcmBackend.Infrastructure.Services;
using QcmBackend.Tests.Common.Moqs;

namespace QcmBackend.Application.FunctionalTests;

public class ApplicationTestFixture
{
    public ServiceProvider ServiceProvider { get; }
    private readonly string _dbName = "TestDb_" + Guid.NewGuid().ToString();

    public ApplicationTestFixture()
    {
        ServiceCollection services = new();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        _ = services.AddSingleton<IConfiguration>(configuration);

        _ = services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        _ = services.Configure<GeneralSettings>(configuration.GetSection("General"));
        _ = services.Configure<SecuritySettings>(configuration.GetSection("Security"));

        _ = services.AddApplicationServices();

        _ = services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        _ = services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();
        _ = services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        _ = services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        _ = services.AddDbContext<AppDbContext>((sp, options) =>
        {
            _ = options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            _ = options.UseSqlite($"DataSource=file:memdb{_dbName}?mode=memory&cache=shared");
            _ = options.ConfigureWarnings(warnings => 
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        _ = services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        _ = services.AddScoped<AppDbContextInitializer>();

        _ = services.AddSingleton(TimeProvider.System);
        _ = services.AddDataProtection();

        _ = services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        _ = services.AddScoped<ITokenService, TokenService>();
        _ = services.AddScoped<IIdentityService, IdentityService>();

        _ = services.AddScoped<ICookieService, FakeCookieService>();
        _ = services.AddScoped<IUser, FakeCurrentUser>();

        ServiceProvider = services.BuildServiceProvider();

        AppDbContextInitializer initialiser = ServiceProvider.GetRequiredService<AppDbContextInitializer>();

        UserManager<AppUser> userManager = ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        IUser userContext = ServiceProvider.GetRequiredService<IUser>();

        initialiser.InitialiseAsync().Wait();
        initialiser.SeedAccountAsync().Wait();

        string adminEmail = "admin@example.com";
        AppUser? adminUser = userManager.FindByEmailAsync(adminEmail).Result ?? throw new Exception("Admin user not found!");

        // using (userContext.BeginScope(adminUser.Id))
        // {
        //     initialiser.SeedAsync().Wait();
        // }
    }
}
