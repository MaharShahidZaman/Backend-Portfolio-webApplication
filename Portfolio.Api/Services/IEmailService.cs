using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public interface IEmailService
{
    Task SendContactNotificationAsync(ContactMessage contactMessage, CancellationToken cancellationToken);
}
