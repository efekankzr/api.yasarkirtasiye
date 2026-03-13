using MediatR;
using api.yasarkirtasiye.Application.Features.Products.DTOs;

namespace api.yasarkirtasiye.Application.Features.Products.Queries.GetSimilarProducts;

public class GetSimilarProductsQuery : IRequest<IEnumerable<ProductDto>>
{
    public Guid ProductId { get; set; }
    public Guid CategoryId { get; set; }
    public int Count { get; set; } = 4;
}
