using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace QcmBackend.Infrastructure.Data.Interceptors
{
    // Interceptor générique pour soft delete
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            MarkSoftDeleted(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            MarkSoftDeleted(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void MarkSoftDeleted(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is ISoftDeleteEntity && e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                entry.State = EntityState.Modified;
                ((ISoftDeleteEntity)entry.Entity).IsDeleted = true;
            }
        }
    }

    // Interface à implémenter sur les entités soft deletables
    public interface ISoftDeleteEntity
    {
        bool IsDeleted { get; set; }
    }
}