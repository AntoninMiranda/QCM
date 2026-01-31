namespace QcmBackend.Application.Common.Settings
{
    public class AuthSettings
    {
        public string SigningKey { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudience { get; set; } = string.Empty;
    }
}
