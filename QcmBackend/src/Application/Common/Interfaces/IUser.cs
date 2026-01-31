using System;
using System.Collections.Generic;

namespace QcmBackend.Application.Common.Interfaces
{
    public interface IUser
    {
        Guid UserId { get; }
        IEnumerable<string> Roles { get; }
        bool HasContext { get; }
        IDisposable BeginScope(Guid userId);
    }
}
