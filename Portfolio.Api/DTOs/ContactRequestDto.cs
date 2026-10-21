using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.DTOs;

public sealed class ContactRequestDto
{
    [Required, StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(200)]
    public string Subject { get; init; } = string.Empty;

    [Required, StringLength(5000, MinimumLength = 10)]
    public string Message { get; init; } = string.Empty;
}
