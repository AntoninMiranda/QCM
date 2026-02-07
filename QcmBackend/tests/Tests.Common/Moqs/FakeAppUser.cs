using QcmBackend.Domain.Interfaces;

namespace QcmBackend.Tests.Common.Moqs
{
    public class FakeAppUser() : IAppUser
    {
        public Guid Id { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? RefreshTokenHash { get; set; }
        public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
        public string? PreviousRefreshTokenHash { get; set; }
        public DateTimeOffset? PreviousRefreshTokenValidUntil { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}