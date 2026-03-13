using MediatR;
using Microsoft.AspNetCore.Mvc;
using api.yasarkirtasiye.Application.Features.Categories.Commands;
using api.yasarkirtasiye.Application.Features.Categories.DTOs;
using api.yasarkirtasiye.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Authorization;

namespace api.yasarkirtasiye.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryCommand command)
    {
        var category = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand { Id = id });
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        var result = await _mediator.Send(new ImportCategoriesCommand { File = file });
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
        var content = await _mediator.Send(new ExportCategoryTemplateQuery());
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kategori_Sablonu.xlsx");
    }
}
