using System;
using System.Collections.Generic;
using QcmBackend.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace QcmBackend.Domain.Common
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private readonly List<BaseEvent> _domainEvents = [];

        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseEvent domainEvent)
        {
            _ = _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
