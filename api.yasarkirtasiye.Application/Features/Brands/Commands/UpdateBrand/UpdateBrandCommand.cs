using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace api.yasarkirtasiye.Application.Features.Brands.Commands.UpdateBrand;

public class UpdateBrandCommand : IRequest<BrandDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    public bool RemoveImage { get; set; }
    public bool IsShowMainPage { get; set; }
}
