using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using QcmBackend.Application.Common.Interfaces;

namespace QcmBackend.Infrastructure.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    private static readonly AsyncLocal<Guid?> _override = new();

    private Guid GetGuidValueOrEmpty(string claimType)
    {
        string? value = httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);

        return value == null || !Guid.TryParse(value, out Guid result) ? Guid.Empty : result;
    }

    public Guid UserId => _override.Value ?? GetGuidValueOrEmpty(ClaimTypes.NameIdentifier);
    public IEnumerable<string> Roles => httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
    public bool HasContext => httpContextAccessor.HttpContext != null || _override.Value != null;

    public IDisposable BeginScope(Guid userId)
    {
        Guid? previous = _override.Value;

        _override.Value = userId;

        return new DisposeAction(() => _override.Value = previous);
    }

    private sealed class DisposeAction(Action a) : IDisposable
    {
        private readonly Action _a = a;

        public void Dispose()
        {
            _a();
        }
    }
}
