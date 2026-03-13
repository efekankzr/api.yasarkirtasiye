using MediatR;
using api.yasarkirtasiye.Application.Features.Products.DTOs;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Wrappers;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Application.Features.Products.Queries;

public class GetAllProductsQuery : IRequest<PagedResult<ProductDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public bool? IsBestSeller { get; set; }
    public bool? IsFeatured { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IRepository<Product> _productRepository;

    public GetAllProductsQueryHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var pagedData = await _productRepository.GetPagedAsync(
            request.PageNumber, 
            request.PageSize, 
            cancellationToken,
            p => (!request.CategoryId.HasValue || p.CategoryId == request.CategoryId.Value) &&
                 (!request.BrandId.HasValue || p.BrandId == request.BrandId.Value) &&
                 (!request.IsBestSeller.HasValue || p.IsBestSeller == request.IsBestSeller.Value) &&
                 (!request.IsFeatured.HasValue || p.IsFeatured == request.IsFeatured.Value) &&
                 (string.IsNullOrEmpty(request.SearchTerm) || 
                  p.Name.ToLower().Contains(request.SearchTerm.ToLower()) ||
                  (p.Brand != null && p.Brand.Name.ToLower().Contains(request.SearchTerm.ToLower())) ||
                  (p.Barcode != null && p.Barcode.ToLower().Contains(request.SearchTerm.ToLower()))),
            p => p.Images,
            p => p.Category,
            p => p.Brand);

        var dtos = pagedData.Items.Select(p => new ProductDto
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

        return new PagedResult<ProductDto>(dtos, pagedData.TotalCount, request.PageNumber, request.PageSize);
    }
}
