namespace api.yasarkirtasiye.Application.Features.Products.DTOs;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
