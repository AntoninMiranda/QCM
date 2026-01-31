using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QcmBackend.Domain.Interfaces;

namespace QcmBackend.Infrastructure.Data.Interceptors
{
    // Interceptor générique pour dispatcher les Domain Events après SaveChanges
    public class DispatchDomainEventsInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
    {
        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            DispatchEvents(eventData.Context);
            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            DispatchEvents(eventData.Context);
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private void DispatchEvents(DbContext? context)
        {
            if (context == null) return;

            var entitiesWithEvents = context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is IEntity entity && entity.DomainEvents.Any())
                .Select(e => e.Entity as IEntity)
                .ToList();

            var events = entitiesWithEvents
                .SelectMany(e => e!.DomainEvents)
                .ToList();

            foreach (var entity in entitiesWithEvents)
            {
                entity!.ClearDomainEvents();
            }

            if (events.Any())
            {
                dispatcher.DispatchAsync(events).GetAwaiter().GetResult();
            }
        }
    }

    // Interface à implémenter pour dispatcher les Domain Events
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<object> events);
    }
}