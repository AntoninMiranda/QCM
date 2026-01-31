using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace QcmBackend.Infrastructure.Data.Interceptors
{
    // Interceptor générique pour audit (Created/Updated)
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var auditable = (IAuditableEntity)entry.Entity;
                var now = DateTimeOffset.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = now;
                    auditable.CreatedBy = "system"; // Remplacer par userId si disponible
                }
                auditable.UpdatedAt = now;
                auditable.UpdatedBy = "system"; // Remplacer par userId si disponible
            }
        }
    }

    // Interface à implémenter sur les entités auditables
    public interface IAuditableEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        string CreatedBy { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
        string UpdatedBy { get; set; }
    }
}
