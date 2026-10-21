using Portfolio.Api.DTOs;

namespace Portfolio.Api.Services;

public interface IContactService
{
    Task ProcessAsync(ContactRequestDto request, CancellationToken cancellationToken);
}
