using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Interfaces.Services;
using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Application.Features.Products.Commands;

public class ExistingImageOrderDto
{
    public Guid Id { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateProductCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsFeatured { get; set; }
    public int PackageQuantity { get; set; } = 1;
    public int BoxQuantity { get; set; } = 1;
    
    // Upload new files
    public List<IFormFile>? NewImages { get; set; }
    
    // JSON array of ExistingImageOrderDto representing images to keep and their new order
    public string? ExistingImagesJson { get; set; } 
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;

    public UpdateProductCommandHandler(IRepository<Product> productRepository, IUnitOfWork unitOfWork, IFileService fileService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken, p => p.Images);
            
        if (product == null)
            return false;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Barcode = request.Barcode;
        product.CategoryId = request.CategoryId;
        product.BrandId = request.BrandId;
        product.IsBestSeller = request.IsBestSeller;
        product.IsFeatured = request.IsFeatured;
        product.PackageQuantity = request.PackageQuantity;
        product.BoxQuantity = request.BoxQuantity;

        // 1. Process Existing Images (Keep & Reorder, or Delete)
        var keptImages = new List<ExistingImageOrderDto>();
        if (!string.IsNullOrEmpty(request.ExistingImagesJson))
        {
            try {
                keptImages = JsonSerializer.Deserialize<List<ExistingImageOrderDto>>(request.ExistingImagesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ExistingImageOrderDto>();
            } catch { /* Ignore parse errors for safety */ }
        }

        var keptImageIds = keptImages.Select(k => k.Id).ToList();
        var imagesToRemove = product.Images.Where(i => !keptImageIds.Contains(i.Id)).ToList();

        // Remove physically and from DB
        foreach (var imgToRemove in imagesToRemove)
        {
            _fileService.DeleteFile(imgToRemove.ImagePath);
            product.Images.Remove(imgToRemove);
        }

        // Update orders of kept images
        foreach (var kept in keptImages)
        {
            var dbImg = product.Images.FirstOrDefault(i => i.Id == kept.Id);
            if (dbImg != null)
            {
                dbImg.DisplayOrder = kept.DisplayOrder;
            }
        }

        var newUploadedPaths = new List<string>();

        // 2. Process New Images
        if (request.NewImages != null && request.NewImages.Any())
        {
            // Find max display order to append
            int maxOrder = product.Images.Any() ? product.Images.Max(i => i.DisplayOrder) : 0;
            
            foreach (var newImg in request.NewImages)
            {
                var imagePath = await _fileService.UploadFileAsync(newImg, "products", cancellationToken);
                newUploadedPaths.Add(imagePath);
                
                maxOrder++;
                product.Images.Add(new ProductImage
                {
                    ImagePath = imagePath,
                    DisplayOrder = maxOrder
                });
            }
        }

        try 
        {
            await _productRepository.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch 
        {
            // Rollback newly uploaded files if DB save fails
            foreach (var path in newUploadedPaths)
            {
                _fileService.DeleteFile(path);
            }
            throw;
        }

        return true;
    }
}
