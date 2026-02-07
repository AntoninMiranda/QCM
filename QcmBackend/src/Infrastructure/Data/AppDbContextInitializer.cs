using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Infrastructure.Identity;
using QcmBackend.Application.Common.Settings;

namespace QcmBackend.Infrastructure.Data
{
    public static class InitializerExtensions
    {
        private static async Task SeedAsync(IServiceScope scope, AppDbContextInitializer initialiser)
        {
            UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            IUser userContext = scope.ServiceProvider.GetRequiredService<IUser>();

            await initialiser.SeedAccountAsync();

            string adminEmail = "admin@example.com";
            AppUser? adminUser = await userManager.FindByEmailAsync(adminEmail) ?? throw new Exception("Admin user not found!");

            // using (userContext.BeginScope(adminUser.Id))
            // {
            //     await initialiser.SeedAsync();
            // }
        }

        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            AppDbContextInitializer initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

            await initialiser.InitialiseAsync();

            await initialiser.SeedAccountAsync();

            await SeedAsync(scope, initialiser);
        }

        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            AppDbContextInitializer initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

            await initialiser.SeedAccountAsync();

            await SeedAsync(scope, initialiser);
        }
    }
    
    public class AppDbContextInitializer(ILogger<AppDbContextInitializer> logger, AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        public async Task InitialiseAsync()
        {
            try
            {
                await context.Database.MigrateAsync(); // Applique les migrations
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAccountAsync()
        {
            try
            {
                await SeedRolesAsync();
                await SeedAdminAccountAsync();
                await SeedUserAccountAsync();
                // Ajoute ici tes données de seed si besoin
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            string[] roleNames = ["Admin", "User"];

            foreach (string roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    _ = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }

        private async Task SeedAdminAccountAsync()
        {
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Account"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
        
        private async Task SeedUserAccountAsync()
        {
            var userEmail = "user@example.com";
            var userUser = await userManager.FindByEmailAsync(userEmail);
            if (userUser == null)
            {
                userUser = new AppUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "User",
                    LastName = "Account"
                };
                var result = await userManager.CreateAsync(userUser, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(userUser, "User");
                }
            }
        }
    }
}