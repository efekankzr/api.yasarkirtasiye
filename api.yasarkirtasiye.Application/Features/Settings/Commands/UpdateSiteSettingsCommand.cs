using MediatR;
using FluentValidation;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Application.Features.Settings.Commands;

public class UpdateSiteSettingsCommand : IRequest<bool>
{
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? WhatsAppNumber { get; set; }
    public string? WhatsAppTemplate { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? InstagramUrl { get; set; }
}

public class UpdateSiteSettingsCommandValidator : AbstractValidator<UpdateSiteSettingsCommand>
{
    public UpdateSiteSettingsCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Geçerli bir e-posta adresi giriniz.");
    }
}

public class UpdateSiteSettingsCommandHandler : IRequestHandler<UpdateSiteSettingsCommand, bool>
{
    private readonly IRepository<SiteSetting> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSiteSettingsCommandHandler(IRepository<SiteSetting> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSiteSettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetByIdAsync(Guid.Parse("11111111-1111-1111-1111-111111111111"), cancellationToken);
        
        if (settings == null) return false;

        settings.Description = request.Description;
        settings.Phone = request.Phone;
        settings.WhatsAppNumber = request.WhatsAppNumber;
        settings.WhatsAppTemplate = request.WhatsAppTemplate;
        settings.Email = request.Email;
        settings.Address = request.Address;
        settings.LogoUrl = request.LogoUrl;
        settings.FacebookUrl = request.FacebookUrl;
        settings.TwitterUrl = request.TwitterUrl;
        settings.InstagramUrl = request.InstagramUrl;

        await _repository.UpdateAsync(settings, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
