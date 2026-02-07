namespace QcmBackend.Application.Common.Settings;

public class AuthSettings
{
    public required string SigningKey { get; set; } = string.Empty;
    public required string ValidIssuer { get; set; } = string.Empty;
    public required string ValidAudience { get; set; } = string.Empty;
    public required int ExpiresInMinutes { get; init; }
    public required int DefaultRefreshExpiresInDays { get; init; }
    public required int RememberMeRefreshExpiresInDays { get; init; }
}

