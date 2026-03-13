using FluentValidation;

namespace api.yasarkirtasiye.Application.Features.Products.Commands;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı boş olamaz.")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Kategori seçilmesi zorunludur.");

        RuleFor(x => x.PackageQuantity)
            .GreaterThan(0).WithMessage("Paket içi adet 0'dan büyük olmalıdır.");

        RuleFor(x => x.BoxQuantity)
            .GreaterThan(0).WithMessage("Koli içi adet 0'dan büyük olmalıdır.");

        RuleFor(x => x)
            .Must(x => 
            {
                var existingCount = 0;
                if (!string.IsNullOrEmpty(x.ExistingImagesJson))
                {
                    try {
                        var keptImages = System.Text.Json.JsonSerializer.Deserialize<List<ExistingImageOrderDto>>(x.ExistingImagesJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        existingCount = keptImages?.Count ?? 0;
                    } catch { } // Ignore parse exception for validation count
                }
                var newCount = x.NewImages?.Count ?? 0;
                return existingCount + newCount <= 3;
            })
            .WithMessage("Bir ürüne en fazla 3 adet görsel eklenebilir.");

        RuleForEach(x => x.NewImages)
            .Must(image => image == null || image.Length <= 3 * 1024 * 1024)
            .WithMessage("Görsel boyutu en fazla 3MB olabilir.");
    }
}
