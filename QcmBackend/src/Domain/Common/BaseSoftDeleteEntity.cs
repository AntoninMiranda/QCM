using System;
using QcmBackend.Domain.Interfaces;

namespace QcmBackend.Domain.Common
{
    public abstract class BaseSoftDeleteEntity : BaseEntity, ISoftDeleteEntity
    {
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedAt { get; set; } = null;
        public Guid? DeletedById { get; set; } = null;
    }
}
