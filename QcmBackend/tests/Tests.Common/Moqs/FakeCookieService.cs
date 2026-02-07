using QcmBackend.Application.Common.Interfaces;

namespace QcmBackend.Tests.Common.Moqs
{
    public class FakeCookieService : ICookieService
    {
        public void RemoveRefreshToken() { }
        public void SetRefreshToken(string refreshToken, DateTimeOffset refreshTokenExpiryDate) { }
        public string? GetRefreshToken() { return null; }
    }
}