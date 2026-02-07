namespace QcmBackend.Application.Common.Settings;

public class AccountSettings
{
    public required string AdminEmail { get; set; } = "admin@exemple.com";
    public required string AdminPassword { get; set; } = "Admin123!";
    public required string UserEmail { get; set; } = "user@exemple.com";
    public required string UserPassword { get; set; } = "User123!";
}