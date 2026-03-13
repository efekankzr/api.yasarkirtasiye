using MediatR;
using api.yasarkirtasiye.Application.Features.Settings.DTOs;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Application.Features.Settings.Queries;

public class GetSiteSettingsQuery : IRequest<SiteSettingDto?>
{
}

public class GetSiteSettingsQueryHandler : IRequestHandler<GetSiteSettingsQuery, SiteSettingDto?>
{
    private readonly IRepository<SiteSetting> _repository;

    public GetSiteSettingsQueryHandler(IRepository<SiteSetting> repository)
    {
        _repository = repository;
    }

    public async Task<SiteSettingDto?> Handle(GetSiteSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetByIdAsync(Guid.Parse("11111111-1111-1111-1111-111111111111"), cancellationToken);
        
        if (settings == null) return null;

        return new SiteSettingDto
        {
            Id = settings.Id,
            Description = settings.Description,
            Phone = settings.Phone,
            Email = settings.Email,
            Address = settings.Address,
            FacebookUrl = settings.FacebookUrl,
            TwitterUrl = settings.TwitterUrl,
            InstagramUrl = settings.InstagramUrl
        };
    }
}
