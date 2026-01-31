using System;

namespace QcmBackend.Domain.Interfaces
{
    public interface ISoftDeleteEntity : IEntity
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeletedAt { get; set; }
        Guid? DeletedById { get; set; }
    }
}
