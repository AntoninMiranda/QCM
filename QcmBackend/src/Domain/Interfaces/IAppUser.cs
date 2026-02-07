using System;

namespace QcmBackend.Domain.Interfaces;

public interface IAppUser
{
    Guid Id { get; set; }
    string? Email { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string? RefreshTokenHash { get; set; }
    DateTimeOffset? RefreshTokenExpiryTime { get; set; }
    string? PreviousRefreshTokenHash { get; set; }
    DateTimeOffset? PreviousRefreshTokenValidUntil { get; set; }
    DateTimeOffset CreatedAt { get; set; }
}