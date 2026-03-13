using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using MediatR;

namespace api.yasarkirtasiye.Application.Features.Brands.Queries.GetAllBrands;

public class GetAllBrandsQuery : IRequest<IEnumerable<BrandDto>>
{
}
