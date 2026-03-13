using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Domain.Entities;
using MediatR;

namespace api.yasarkirtasiye.Application.Features.Brands.Queries.GetBrandById;

public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, BrandDto>
{
    private readonly IRepository<Brand> _brandRepository;

    public GetBrandByIdQueryHandler(IRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<BrandDto> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);

        if (brand == null)
            throw new Exception("Brand not found.");

        return new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            ImageUrl = brand.ImageUrl,
            IsShowMainPage = brand.IsShowMainPage
        };
    }
}
