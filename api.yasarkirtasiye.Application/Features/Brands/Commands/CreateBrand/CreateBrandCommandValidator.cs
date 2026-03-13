using FluentValidation;

namespace api.yasarkirtasiye.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Marka adı boş olamaz.")
            .MaximumLength(100).WithMessage("Marka adı en fazla 100 karakter olabilir.");
    }
}
