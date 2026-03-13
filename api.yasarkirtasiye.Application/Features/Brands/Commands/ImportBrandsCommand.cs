using api.yasarkirtasiye.Application.Common.Models;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace api.yasarkirtasiye.Application.Features.Brands.Commands;

public class ImportBrandsCommand : IRequest<ImportResult>
{
    public IFormFile File { get; set; } = null!;
}

public class ImportBrandsCommandHandler : IRequestHandler<ImportBrandsCommand, ImportResult>
{
    private readonly IRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportBrandsCommandHandler(IRepository<Brand> brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ImportResult> Handle(ImportBrandsCommand request, CancellationToken cancellationToken)
    {
        var result = new ImportResult();
        
        if (request.File == null || request.File.Length == 0)
        {
            result.Errors.Add("Dosya bulunamadı veya boş.");
            return result;
        }
        
        var ext = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (ext != ".xlsx")
        {
            result.Errors.Add("Sadece .xlsx formatında Excel dosyaları desteklenmektedir.");
            return result;
        }

        using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream, cancellationToken);
        
        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        
        if (worksheet == null)
        {
            result.Errors.Add("Excel dosyasında sayfa bulunamadı.");
            return result;
        }
        
        var rowCount = worksheet.Dimension?.Rows ?? 0;
        if (rowCount < 2)
        {
            result.Errors.Add("Excel dosyasında eklenecek veri bulunamadı.");
            return result;
        }
        
        var existingBrands = await _brandRepository.GetAllAsync();
        var existingNames = existingBrands.Select(b => b.Name.ToLowerInvariant()).ToHashSet();
        
        for (int row = 2; row <= rowCount; row++)
        {
            var name = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
            
            if (string.IsNullOrWhiteSpace(name))
                continue;
                
            if (existingNames.Contains(name.ToLowerInvariant()))
            {
                result.Errors.Add($"Satır {row}: '{name}' adında bir marka zaten mevcut.");
                continue;
            }
            
            var brand = new Brand
            {
                Name = name
            };
            
            await _brandRepository.AddAsync(brand);
            existingNames.Add(name.ToLowerInvariant());
            result.SuccessCount++;
        }
        
        if (result.SuccessCount > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        
        return result;
    }
}
