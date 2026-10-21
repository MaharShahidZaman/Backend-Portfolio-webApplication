using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.DTOs;
using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public sealed class ContactService(
    ApplicationDbContext dbContext,
    IEmailService emailService,
    ILogger<ContactService> logger) : IContactService
{
    public async Task ProcessAsync(ContactRequestDto request, CancellationToken cancellationToken)
    {
        var contactMessage = new ContactMessage
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        dbContext.ContactMessages.Add(contactMessage);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Saved contact message {ContactMessageId} from {SenderEmail}.", contactMessage.Id, contactMessage.Email);

        try
        {
            await emailService.SendContactNotificationAsync(contactMessage, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // The message remains safely stored even if notification delivery is temporarily unavailable.
            logger.LogError(exception, "Email notification failed for saved contact message {ContactMessageId}.", contactMessage.Id);
        }
    }
}
