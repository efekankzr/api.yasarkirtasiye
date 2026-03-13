using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Domain.Entities;
using MediatR;

namespace api.yasarkirtasiye.Application.Features.Brands.Queries.GetAllBrands;

public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, IEnumerable<BrandDto>>
{
    private readonly IRepository<Brand> _brandRepository;

    public GetAllBrandsQueryHandler(IRepository<Brand> brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<IEnumerable<BrandDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var brandsResult = await _brandRepository.GetPagedAsync(1, 1000, cancellationToken);
        var brands = brandsResult.Items.OrderBy(b => b.Name).ToList();

        return brands.Select(b => new BrandDto
        {
            Id = b.Id,
            Name = b.Name,
            ImageUrl = b.ImageUrl,
            IsShowMainPage = b.IsShowMainPage
        });
    }
}
