using api.yasarkirtasiye.Application.Common.Models;
using api.yasarkirtasiye.Application.Interfaces.Repositories;
using api.yasarkirtasiye.Application.Interfaces;
using api.yasarkirtasiye.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace api.yasarkirtasiye.Application.Features.Categories.Commands;

public class ImportCategoriesCommand : IRequest<ImportResult>
{
    public IFormFile File { get; set; } = null!;
}

public class ImportCategoriesCommandHandler : IRequestHandler<ImportCategoriesCommand, ImportResult>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportCategoriesCommandHandler(IRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ImportResult> Handle(ImportCategoriesCommand request, CancellationToken cancellationToken)
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
            result.Errors.Add("Excel dosyasında eklenecek veri bulunamadı (Başlık satırından sonrası boş).");
            return result;
        }
        
        var existingCategories = await _categoryRepository.GetAllAsync();
        var existingNames = existingCategories.Select(c => c.Name.ToLowerInvariant()).ToHashSet();
        
        for (int row = 2; row <= rowCount; row++)
        {
            var name = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
            
            if (string.IsNullOrWhiteSpace(name))
                continue;
                
            if (existingNames.Contains(name.ToLowerInvariant()))
            {
                result.Errors.Add($"Satır {row}: '{name}' adında bir kategori zaten mevcut.");
                continue;
            }
            
            var slug = GenerateSlug(name);
            var category = new Category
            {
                Name = name,
                Slug = slug
            };
            
            await _categoryRepository.AddAsync(category);
            existingNames.Add(name.ToLowerInvariant());
            result.SuccessCount++;
        }
        
        if (result.SuccessCount > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        
        return result;
    }
    
    private string GenerateSlug(string phrase)
    {
        string str = phrase.ToLowerInvariant();
        str = str.Replace("ö", "o").Replace("ü", "u").Replace("ı", "i").Replace("ş", "s").Replace("ç", "c").Replace("ğ", "g");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim();
        return str.Replace(" ", "-");
    }
}
