namespace api.yasarkirtasiye.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsFeatured { get; set; }
    
    public int PackageQuantity { get; set; } = 1;
    public int BoxQuantity { get; set; } = 1;
    
    // Foreign Key
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    
    // Navigation Property
    public Category Category { get; set; } = null!;
    public Brand? Brand { get; set; }
    
    // Images
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
