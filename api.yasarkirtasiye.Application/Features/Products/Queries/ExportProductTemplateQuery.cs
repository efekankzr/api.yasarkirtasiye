using MediatR;
using OfficeOpenXml;

namespace api.yasarkirtasiye.Application.Features.Products.Queries;

public class ExportProductTemplateQuery : IRequest<byte[]>
{
}

public class ExportProductTemplateQueryHandler : IRequestHandler<ExportProductTemplateQuery, byte[]>
{
    public Task<byte[]> Handle(ExportProductTemplateQuery request, CancellationToken cancellationToken)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Ürünler");
        
        var headers = new[] {
            "Satılacak Ürün Adı", "Kategori Adı", "Marka Adı (Opsiyonel)", "Barkod Numarası", 
            "Ürün Açıklaması", "Koli İçi Adet", "Paket İçi Adet", "Çok Satan (E/H)", 
            "Öne Çıkan (E/H)", "Görseller(Virgülle Ayırın)"
        };
        
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
        }
        
        worksheet.Cells["A:J"].AutoFitColumns(15);
        
        return Task.FromResult(package.GetAsByteArray());
    }
}
