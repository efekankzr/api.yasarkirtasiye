using Microsoft.AspNetCore.Http;

namespace api.yasarkirtasiye.Application.Interfaces.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string subDirectory, CancellationToken cancellationToken = default);
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string subDirectory, CancellationToken cancellationToken = default);
    bool DeleteFile(string filePath);
}
