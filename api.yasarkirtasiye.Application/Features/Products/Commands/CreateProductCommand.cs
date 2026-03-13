using MediatR;
using Microsoft.AspNetCore.Http;
using api.yasarkirtasiye.Application.Features.Products.DTOs;
using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Interfaces.Services;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsFeatured { get; set; }
    public int PackageQuantity { get; set; } = 1;
    public int BoxQuantity { get; set; } = 1;
    
    public List<IFormFile>? Images { get; set; } 
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public CreateProductCommandHandler(IRepository<Product> productRepository, IUnitOfWork unitOfWork, IFileService fileService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Barcode = request.Barcode,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            IsBestSeller = request.IsBestSeller,
            IsFeatured = request.IsFeatured,
            PackageQuantity = request.PackageQuantity,
            BoxQuantity = request.BoxQuantity
        };

        if (request.Images != null && request.Images.Any())
        {
            var displayOrder = 1;
            foreach (var img in request.Images)
            {
                var imagePath = await _fileService.UploadFileAsync(img, "products", cancellationToken);
                product.Images.Add(new ProductImage
                {
                    ImagePath = imagePath,
                    DisplayOrder = displayOrder++
                });
            }
        }

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Barcode = product.Barcode,
            IsBestSeller = product.IsBestSeller,
            IsFeatured = product.IsFeatured,
            PackageQuantity = product.PackageQuantity,
            BoxQuantity = product.BoxQuantity,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            Images = product.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImagePath = i.ImagePath,
                DisplayOrder = i.DisplayOrder
            }).ToList()
        };
    }
}
