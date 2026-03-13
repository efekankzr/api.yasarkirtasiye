using FluentValidation;

namespace api.yasarkirtasiye.Application.Features.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
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

        RuleFor(x => x.Images)
            .Must(images => images == null || images.Count <= 3)
            .WithMessage("Bir ürüne en fazla 3 adet görsel eklenebilir.");

        RuleForEach(x => x.Images)
            .Must(image => image == null || image.Length <= 3 * 1024 * 1024)
            .WithMessage("Görsel boyutu en fazla 3MB olabilir.");
    }
}
