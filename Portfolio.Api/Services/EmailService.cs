using Microsoft.Extensions.Options;
using Portfolio.Api.Models;
using Portfolio.Api.Settings;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Portfolio.Api.Services;

public sealed class EmailService(
    HttpClient httpClient,
    IOptions<ResendSettings> resendOptions,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly ResendSettings _settings = resendOptions.Value;

    public async Task SendContactNotificationAsync(ContactMessage contactMessage, CancellationToken cancellationToken)
    {
        ValidateConfiguration();

        var payload = new
        {
            from = _settings.FromEmail,
            to = new[] { _settings.ToEmail },
            subject = $"New Portfolio Contact: {contactMessage.Subject}",
            html = BuildHtmlBody(contactMessage)
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Resend returned non-success status code {StatusCode} for contact message {ContactMessageId}.",
                    (int)response.StatusCode, contactMessage.Id);
                throw new HttpRequestException("The email notification provider returned an unsuccessful response.", null, response.StatusCode);
            }

            logger.LogInformation("Email notification sent for contact message {ContactMessageId}.", contactMessage.Id);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Resend email request failed for contact message {ContactMessageId}.", contactMessage.Id);
            throw;
        }
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            string.IsNullOrWhiteSpace(_settings.FromEmail) ||
            string.IsNullOrWhiteSpace(_settings.ToEmail))
        {
            throw new InvalidOperationException(
                "Resend notification settings are incomplete. Configure Resend settings before enabling notifications.");
        }
    }

    private static string BuildHtmlBody(ContactMessage contactMessage) => $"""
        <h2>New portfolio contact message</h2>
        <p><strong>Name:</strong> {HtmlEncode(contactMessage.Name)}</p>
        <p><strong>Email:</strong> {HtmlEncode(contactMessage.Email)}</p>
        <p><strong>Subject:</strong> {HtmlEncode(contactMessage.Subject)}</p>
        <p><strong>Submitted at (UTC):</strong> {HtmlEncode(contactMessage.CreatedAt.ToString("O"))}</p>
        <p><strong>Message:</strong><br />{HtmlEncodeWithLineBreaks(contactMessage.Message)}</p>
        """;

    private static string HtmlEncode(string value) => WebUtility.HtmlEncode(value);

    private static string HtmlEncodeWithLineBreaks(string value) =>
        HtmlEncode(value).Replace("\r\n", "<br />", StringComparison.Ordinal).Replace("\n", "<br />", StringComparison.Ordinal);
}
