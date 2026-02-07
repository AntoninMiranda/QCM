using QcmBackend.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace QcmBackend.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>, IAppUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? RefreshTokenHash { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
    public string? PreviousRefreshTokenHash { get; set; }
    public DateTimeOffset? PreviousRefreshTokenValidUntil { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
