namespace api.yasarkirtasiye.Domain.Entities;

public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsShowMainPage { get; set; } = false;

    // Navigation Property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
