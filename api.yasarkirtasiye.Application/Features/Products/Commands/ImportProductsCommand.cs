using api.yasarkirtasiye.Application.Common.Models;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Interfaces.Services;
using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.IO.Compression;

namespace api.yasarkirtasiye.Application.Features.Products.Commands;

public class ImportProductsCommand : IRequest<ImportResult>
{
    public IFormFile File { get; set; } = null!;
}

public class ImportProductsCommandHandler : IRequestHandler<ImportProductsCommand, ImportResult>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Brand> _brandRepository;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;

    public ImportProductsCommandHandler(
        IRepository<Product> productRepository,
        IRepository<Category> categoryRepository,
        IRepository<Brand> brandRepository,
        IFileService fileService,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ImportResult> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        var result = new ImportResult();

        if (request.File == null || request.File.Length == 0)
        {
            result.Errors.Add("Yüklenen dosya boş.");
            return result;
        }

        var ext = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (ext != ".zip")
        {
            result.Errors.Add("Lütfen ürünlerin Excel dosyasını ve resimleri içeren bir .zip dosyası yükleyin.");
            return result;
        }

        var categories = await _categoryRepository.GetAllAsync();
        var brands = await _brandRepository.GetAllAsync();
        
        var categoryDict = categories.ToDictionary(c => c.Name.ToLowerInvariant(), c => c.Id);
        var brandDict = brands.ToDictionary(b => b.Name.ToLowerInvariant(), b => b.Id);

        using var zipStream = new MemoryStream();
        await request.File.CopyToAsync(zipStream, cancellationToken);
        zipStream.Position = 0;

        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        
        var excelEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase));
        if (excelEntry == null)
        {
            result.Errors.Add("ZIP dosyası içinde .xlsx uzantılı bir Excel dosyası bulunamadı.");
            return result;
        }

        using var excelStream = new MemoryStream();
        using (var entryStream = excelEntry.Open())
        {
            await entryStream.CopyToAsync(excelStream, cancellationToken);
        }
        excelStream.Position = 0;

        using var package = new ExcelPackage(excelStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            result.Errors.Add("Excel dosyasında sayfa bulunamadı.");
            return result;
        }

        var rowCount = worksheet.Dimension?.Rows ?? 0;
        if (rowCount < 2)
        {
            result.Errors.Add("Eklenecek ürün verisi bulunamadı.");
            return result;
        }

        for (int row = 2; row <= rowCount; row++)
        {
            var productName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(productName)) continue;

            var categoryName = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
            var brandName = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
            var barcode = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
            var description = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
            
            _ = int.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out int boxQuantity);
            _ = int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out int packageQuantity);
            
            var isBestSellerStr = worksheet.Cells[row, 8].Value?.ToString()?.Trim().ToLowerInvariant();
            var isFeaturedStr = worksheet.Cells[row, 9].Value?.ToString()?.Trim().ToLowerInvariant();
            
            bool isBestSeller = isBestSellerStr == "e" || isBestSellerStr == "evet" || isBestSellerStr == "true";
            bool isFeatured = isFeaturedStr == "e" || isFeaturedStr == "evet" || isFeaturedStr == "true";
            
            var imagesStr = worksheet.Cells[row, 10].Value?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(categoryName) || !categoryDict.TryGetValue(categoryName.ToLowerInvariant(), out Guid categoryId))
            {
                result.Errors.Add($"Satır {row}: Kategori '{categoryName}' sistemde bulunamadı. Lütfen önce kategoriyi ekleyin.");
                continue;
            }

            Guid? brandId = null;
            if (!string.IsNullOrWhiteSpace(brandName))
            {
                if (brandDict.TryGetValue(brandName.ToLowerInvariant(), out Guid bId))
                {
                    brandId = bId;
                }
                else
                {
                    result.Errors.Add($"Satır {row}: Marka '{brandName}' sistemde bulunamadı. Lütfen önce markayı ekleyin veya boş bırakın.");
                    continue;
                }
            }

            var product = new Product
            {
                Name = productName,
                CategoryId = categoryId,
                BrandId = brandId,
                Barcode = barcode,
                Description = description ?? "",
                BoxQuantity = boxQuantity > 0 ? boxQuantity : 1,
                PackageQuantity = packageQuantity > 0 ? packageQuantity : 1,
                IsBestSeller = isBestSeller,
                IsFeatured = isFeatured
            };

            // Process Images
            if (!string.IsNullOrWhiteSpace(imagesStr))
            {
                var imageFiles = imagesStr.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(s => s.Trim())
                                          .ToList();

                int displayOrder = 0;
                foreach (var imgFile in imageFiles)
                {
                    // Find image in ZIP
                    var imgEntry = archive.Entries.FirstOrDefault(e => e.Name.Equals(imgFile, StringComparison.OrdinalIgnoreCase));
                    if (imgEntry != null)
                    {
                        using var imgStream = imgEntry.Open();
                        // Copy to memory stream to upload
                        using var ms = new MemoryStream();
                        await imgStream.CopyToAsync(ms, cancellationToken);
                        ms.Position = 0;

                        try
                        {
                            var savedPath = await _fileService.UploadFileAsync(ms, imgEntry.Name, "products", cancellationToken);
                            product.Images.Add(new ProductImage
                            {
                                ImagePath = savedPath,
                                DisplayOrder = displayOrder++
                            });
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Satır {row}: Görsel kaydedilemedi ({imgFile}) - {ex.Message}");
                        }
                    }
                    else
                    {
                        result.Errors.Add($"Satır {row}: ZIP dosyasında '{imgFile}' görseli bulunamadı.");
                    }
                }
            }

            await _productRepository.AddAsync(product);
            result.SuccessCount++;
        }

        if (result.SuccessCount > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return result;
    }
}
