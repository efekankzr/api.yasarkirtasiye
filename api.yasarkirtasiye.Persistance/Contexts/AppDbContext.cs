using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Persistance.Contexts;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductImage> ProductImages { get; set; } = null!;
    public DbSet<SiteSetting> SiteSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Seed default site settings
        modelBuilder.Entity<SiteSetting>().HasData(new SiteSetting
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Description = "Kaliteli kırtasiye malzemeleri, toptan kırtasiye çözümleri ve daha fazlası. Geniş ürün yelpazemiz için bizimle iletişime geçin.",
            Phone = "0 (555) 123 45 67",
            Email = "info@yasarkirtasiye.com",
            Address = "Merkez Mah. Atatürk Cad. No:1 Ankara/Türkiye",
            FacebookUrl = "https://facebook.com",
            TwitterUrl = "https://twitter.com",
            InstagramUrl = "https://instagram.com",
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
