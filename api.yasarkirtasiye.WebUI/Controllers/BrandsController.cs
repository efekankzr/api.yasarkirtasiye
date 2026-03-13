using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.yasarkirtasiye.Application.Features.Brands.Commands;
using api.yasarkirtasiye.Application.Features.Brands.Commands.CreateBrand;
using api.yasarkirtasiye.Application.Features.Brands.Commands.DeleteBrand;
using api.yasarkirtasiye.Application.Features.Brands.Commands.UpdateBrand;
using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using api.yasarkirtasiye.Application.Features.Brands.Queries;
using api.yasarkirtasiye.Application.Features.Brands.Queries.GetAllBrands;
using api.yasarkirtasiye.Application.Features.Brands.Queries.GetBrandById;

namespace api.yasarkirtasiye.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
    {
        var brands = await _mediator.Send(new GetAllBrandsQuery());
        return Ok(brands);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BrandDto>> GetById(Guid id)
    {
        var brand = await _mediator.Send(new GetBrandByIdQuery(id));
        return Ok(brand);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandDto>> Create([FromForm] CreateBrandCommand command)
    {
        var brand = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandDto>> Update(Guid id, [FromForm] UpdateBrandCommand command)
    {
        if (command.Id == Guid.Empty) command.Id = id;
        if (id != command.Id) return BadRequest(new { Message = "ID Uyuşmazlığı" });
        var brand = await _mediator.Send(command);
        return Ok(brand);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteBrandCommand(id));
        return NoContent();
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        var result = await _mediator.Send(new ImportBrandsCommand { File = file });
        if (result.Errors.Any() && result.SuccessCount == 0)
        {
            return BadRequest(new { Message = "İçe aktarma başarısız oldu.", result.Errors });
        }
        return Ok(new { Message = $"{result.SuccessCount} kayıt başarıyla içe aktarıldı.", result.Errors });
    }

    [HttpGet("template")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DownloadTemplate()
    {
        var content = await _mediator.Send(new ExportBrandTemplateQuery());
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Marka_Sablonu.xlsx");
    }
}
