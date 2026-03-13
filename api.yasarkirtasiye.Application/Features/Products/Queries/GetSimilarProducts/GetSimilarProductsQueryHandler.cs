using MediatR;
using api.yasarkirtasiye.Application.Features.Products.DTOs;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Application.Features.Products.Queries.GetSimilarProducts;

public class GetSimilarProductsQueryHandler : IRequestHandler<GetSimilarProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IRepository<Product> _productRepository;

    public GetSimilarProductsQueryHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetSimilarProductsQuery request, CancellationToken cancellationToken)
    {
        var pagedData = await _productRepository.GetPagedAsync(
            1,
            request.Count,
            cancellationToken,
            p => p.CategoryId == request.CategoryId && p.Id != request.ProductId,
            p => p.Images,
            p => p.Category,
            p => p.Brand
        );

        return pagedData.Items.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Barcode = p.Barcode,
            IsBestSeller = p.IsBestSeller,
            IsFeatured = p.IsFeatured,
            PackageQuantity = p.PackageQuantity,
            BoxQuantity = p.BoxQuantity,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name ?? string.Empty,
            Images = p.Images.OrderBy(i => i.DisplayOrder).Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImagePath = i.ImagePath,
                DisplayOrder = i.DisplayOrder
            }).ToList()
        });
    }
}
