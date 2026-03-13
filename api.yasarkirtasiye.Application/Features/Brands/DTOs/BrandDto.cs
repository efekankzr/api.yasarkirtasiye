namespace api.yasarkirtasiye.Application.Features.Brands.DTOs;

public class BrandDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsShowMainPage { get; set; }
}
