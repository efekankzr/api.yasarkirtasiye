using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Persistance.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(150);

        // Slug unique constraint — iki kategori aynı slug'a sahip olamaz
        builder.HasIndex(c => c.Slug).IsUnique();
    }
}
