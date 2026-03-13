using MediatR;
using OfficeOpenXml;

namespace api.yasarkirtasiye.Application.Features.Brands.Queries;

public class ExportBrandTemplateQuery : IRequest<byte[]>
{
}

public class ExportBrandTemplateQueryHandler : IRequestHandler<ExportBrandTemplateQuery, byte[]>
{
    public Task<byte[]> Handle(ExportBrandTemplateQuery request, CancellationToken cancellationToken)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Markalar");
        
        // Headers
        worksheet.Cells[1, 1].Value = "Marka Adı";
        worksheet.Cells[1, 2].Value = "Açıklama (İsteğe Bağlı)";
        
        // Styling
        worksheet.Cells["A1:B1"].Style.Font.Bold = true;
        worksheet.Cells["A:B"].AutoFitColumns(20);
        
        return Task.FromResult(package.GetAsByteArray());
    }
}
