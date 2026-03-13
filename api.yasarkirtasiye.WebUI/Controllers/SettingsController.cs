using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.yasarkirtasiye.Application.Features.Settings.Queries;
using api.yasarkirtasiye.Application.Features.Settings.Commands;

namespace api.yasarkirtasiye.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous] // Public access for the storefront footer
    public async Task<IActionResult> GetSettings()
    {
        var result = await _mediator.Send(new GetSiteSettingsQuery());
        if (result == null) return NotFound("Ayarlar bulunamadı.");
        
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")] // Only Admin can update
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSiteSettingsCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result) return BadRequest("Ayarlar güncellenemedi.");

        return Ok(new { message = "Ayarlar başarıyla güncellendi." });
    }
}
