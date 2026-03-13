using Microsoft.AspNetCore.Identity;

namespace api.yasarkirtasiye.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // CreatedAt and UpdatedAt are manually added since we dropped BaseEntity
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
