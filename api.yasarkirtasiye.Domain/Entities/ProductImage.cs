namespace api.yasarkirtasiye.Domain.Entities;

public class ProductImage : BaseEntity
{
    public string ImagePath { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    // Foreign Key
    public Guid ProductId { get; set; }

    // Navigation Property
    public Product Product { get; set; } = null!;
}
