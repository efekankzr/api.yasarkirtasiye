using MediatR;
using Microsoft.AspNetCore.Mvc;
using api.yasarkirtasiye.Application.Features.Products.Commands;
using api.yasarkirtasiye.Application.Features.Products.Queries;
using api.yasarkirtasiye.Application.Features.Products.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace api.yasarkirtasiye.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] GetAllProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery { Id = id });
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("{id}/similar")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetSimilar(Guid id, [FromQuery] Guid categoryId, [FromQuery] int count = 4)
    {
        var query = new api.yasarkirtasiye.Application.Features.Products.Queries.GetSimilarProducts.GetSimilarProductsQuery 
        { 
            ProductId = id, 
            CategoryId = categoryId, 
            Count = count 
        };
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create([FromForm] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductCommand command)
    {
        if (command.Id == Guid.Empty) command.Id = id;
        if (id != command.Id) return BadRequest(new { Message = "ID Uyuşmazlığı" });
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductCommand { Id = id });
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        var result = await _mediator.Send(new ImportProductsCommand { File = file });
        if (result.Errors.Any() && result.SuccessCount == 0)
        {
            return BadRequest(new { Message = "İçe aktarma başarısız oldu.", result.Errors });
        }
        return Ok(new { Message = $"{result.SuccessCount} ürün başarıyla içe aktarıldı.", result.Errors });
    }

    [HttpGet("template")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DownloadTemplate()
    {
        var content = await _mediator.Send(new ExportProductTemplateQuery());
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Urun_Sablonu.xlsx");
    }
}
