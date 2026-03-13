using MediatR;
using OfficeOpenXml;

namespace api.yasarkirtasiye.Application.Features.Categories.Queries;

public class ExportCategoryTemplateQuery : IRequest<byte[]>
{
}

public class ExportCategoryTemplateQueryHandler : IRequestHandler<ExportCategoryTemplateQuery, byte[]>
{
    public Task<byte[]> Handle(ExportCategoryTemplateQuery request, CancellationToken cancellationToken)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Kategoriler");
        
        // Headers
        worksheet.Cells[1, 1].Value = "Kategori Adı";
        
        // Styling
        worksheet.Cells["A1"].Style.Font.Bold = true;
        worksheet.Cells["A:A"].AutoFitColumns(20);
        
        return Task.FromResult(package.GetAsByteArray());
    }
}
