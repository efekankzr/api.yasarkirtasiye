namespace api.yasarkirtasiye.Application.Features.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsFeatured { get; set; }
    public int PackageQuantity { get; set; } = 1;
    public int BoxQuantity { get; set; } = 1;
    
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid? BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public IList<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
}
