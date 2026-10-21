using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/contact")]
public sealed class ContactController(IContactService contactService, ILogger<ContactController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Submit([FromBody] ContactRequestDto request, CancellationToken cancellationToken)
    {
        await contactService.ProcessAsync(request, cancellationToken);
        logger.LogInformation("Contact request completed successfully.");

        return Ok(new { success = true, message = "Your message has been sent successfully." });
    }
}
