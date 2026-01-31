using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QcmBackend.Application.Common.Interfaces;
using QcmBackend.Infrastructure.Identity;

namespace QcmBackend.Infrastructure.Data
{
    // DbContext de base pour EF Core et Identity
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<string>, string>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Ajoute ici tes configurations personnalisées
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}