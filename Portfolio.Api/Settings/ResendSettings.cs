using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Settings;

public sealed class ResendSettings
{
    public const string SectionName = "Resend";

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string FromEmail { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string ToEmail { get; init; } = string.Empty;
}
