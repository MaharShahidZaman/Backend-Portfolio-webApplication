using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "EmailSettings";

    [Required] public string Host { get; init; } = string.Empty;
    [Range(1, 65535)] public int Port { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    [Required, EmailAddress] public string FromEmail { get; init; } = string.Empty;
    [Required, EmailAddress] public string ToEmail { get; init; } = string.Empty;
    public bool UseSsl { get; init; } = true;
}
