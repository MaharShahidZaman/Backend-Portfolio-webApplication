using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Portfolio.Api.Models;
using Portfolio.Api.Settings;

namespace Portfolio.Api.Services;

public sealed class EmailService(
    IOptions<EmailSettings> emailOptions,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = emailOptions.Value;

    public async Task SendContactNotificationAsync(ContactMessage contactMessage, CancellationToken cancellationToken)
    {
        ValidateConfiguration();

        var notification = new MimeMessage();
        notification.From.Add(MailboxAddress.Parse(_settings.FromEmail));
        notification.To.Add(MailboxAddress.Parse(_settings.ToEmail));
        notification.ReplyTo.Add(new MailboxAddress(contactMessage.Name, contactMessage.Email));
        notification.Subject = $"Portfolio contact: {contactMessage.Subject}";

        notification.Body = new TextPart("plain")
        {
            Text = $"A new portfolio contact message was received.\n\n" +
                   $"Name: {contactMessage.Name}\n" +
                   $"Email: {contactMessage.Email}\n" +
                   $"Subject: {contactMessage.Subject}\n" +
                   $"Submitted (UTC): {contactMessage.CreatedAt:O}\n\n" +
                   $"Message:\n{contactMessage.Message}"
        };

        using var client = new SmtpClient();
        var socketOptions = _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await client.ConnectAsync(_settings.Host, _settings.Port, socketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
        }

        await client.SendAsync(notification, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Email notification sent to {Recipient} for contact message from {SenderEmail}.",
            _settings.ToEmail, contactMessage.Email);
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_settings.Host) ||
            _settings.Port is < 1 or > 65535 ||
            string.IsNullOrWhiteSpace(_settings.FromEmail) ||
            string.IsNullOrWhiteSpace(_settings.ToEmail))
        {
            throw new InvalidOperationException(
                "Email notification settings are incomplete. Configure EmailSettings before enabling notifications.");
        }
    }
}
