using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace api.yasarkirtasiye.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommand : IRequest<BrandDto>
{
    public string Name { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    public bool IsShowMainPage { get; set; }
}
