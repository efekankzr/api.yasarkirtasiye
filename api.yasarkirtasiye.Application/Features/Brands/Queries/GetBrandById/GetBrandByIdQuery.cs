using api.yasarkirtasiye.Application.Features.Brands.DTOs;
using MediatR;

namespace api.yasarkirtasiye.Application.Features.Brands.Queries.GetBrandById;

public class GetBrandByIdQuery : IRequest<BrandDto>
{
    public Guid Id { get; set; }

    public GetBrandByIdQuery(Guid id)
    {
        Id = id;
    }
}
