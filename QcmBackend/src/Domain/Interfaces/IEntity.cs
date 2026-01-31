using System;
using System.Collections.Generic;
using QcmBackend.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace QcmBackend.Domain.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }

        [NotMapped]
        IReadOnlyCollection<BaseEvent> DomainEvents { get; }

        void AddDomainEvent(BaseEvent domainEvent);
        void RemoveDomainEvent(BaseEvent domainEvent);
        void ClearDomainEvents();
    }
}
