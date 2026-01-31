using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Infrastructure.Identity;

namespace QcmBackend.Infrastructure.Data
{
    public static class InitializerExtensions
    {
        private static async Task SeedAsync(IServiceScope scope, AppDbContextInitializer initialiser)
        {
            UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            // IUser userContext = scope.ServiceProvider.GetRequiredService<IUser>();

            string adminEmail = "admin@example.com";
            AppUser? adminUser = await userManager.FindByEmailAsync(adminEmail) ?? throw new Exception("Admin user not found!");

            // using (userContext.BeginScope(adminUser.Id))
            // {
                await initialiser.SeedAsync();
            // }
        }

        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            AppDbContextInitializer initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

            await initialiser.InitialiseAsync();

            // await initialiser.SeedUsersAsync();

            await SeedAsync(scope, initialiser);
        }

        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            AppDbContextInitializer initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

            // await initialiser.SeedUsersAsync();

            await SeedAsync(scope, initialiser);
        }
    }
    
    // Utilisé pour initialiser la base au démarrage (migrations, seed)
    
    public class AppDbContextInitializer
    {
        private readonly ILogger<AppDbContextInitializer> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppDbContextInitializer(ILogger<AppDbContextInitializer> logger, AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync(); // Applique les migrations
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedRolesAsync();
                await SeedAdminUserAsync();
                // Ajoute ici tes données de seed si besoin
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = "admin@example.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User"
                };
                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}