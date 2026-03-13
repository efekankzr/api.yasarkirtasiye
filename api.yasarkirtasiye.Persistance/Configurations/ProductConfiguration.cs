using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Persistance.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Description)
            .HasMaxLength(2000);
        builder.Property(p => p.Barcode)
            .HasMaxLength(50);
            
        builder.Property(p => p.PackageQuantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.BoxQuantity)
            .IsRequired()
            .HasDefaultValue(1);
        
        // One-to-Many Relationship: Category -> Products
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
