using Microsoft.AspNetCore.Http;
using api.yasarkirtasiye.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace api.yasarkirtasiye.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _baseUploadPath;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public FileService(IConfiguration configuration)
    {
        // Use wwwroot/uploads natively inside the backend project
        _baseUploadPath = configuration["FileStorage:UploadDirectory"]
                          ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
    }

    public async Task<string> UploadFileAsync(IFormFile file, string subDirectory, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        ValidateFile(file.Length, Path.GetExtension(file.FileName));

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;
        return await SaveStreamAsync(stream, file.FileName, subDirectory, cancellationToken);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string subDirectory, CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("Stream is empty.");

        ValidateFile(fileStream.Length, Path.GetExtension(fileName));

        return await SaveStreamAsync(fileStream, fileName, subDirectory, cancellationToken);
    }

    public bool DeleteFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return false;

        if (filePath.StartsWith("/"))
            filePath = filePath.Substring(1);

        var uploadsIndex = filePath.IndexOf("uploads/");
        if (uploadsIndex >= 0)
        {
            var relative = filePath.Substring(uploadsIndex + "uploads/".Length);
            var absolutePath = Path.Combine(_baseUploadPath, relative);

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
                return true;
            }
        }

        return false;
    }

    // ─── Private Helpers ───────────────────────────────────────────────────────

    private static void ValidateFile(long fileLength, string extension)
    {
        const int maxFileSize = 3 * 1024 * 1024; // 3 MB
        if (fileLength > maxFileSize)
            throw new ArgumentException("Dosya boyutu 3MB'den büyük olamaz.");

        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException($"Geçersiz dosya türü. İzin verilen uzantılar: {string.Join(", ", AllowedExtensions)}");
    }

    private async Task<string> SaveStreamAsync(Stream stream, string originalFileName, string subDirectory, CancellationToken cancellationToken)
    {
        var uploadDir = Path.Combine(_baseUploadPath, subDirectory);

        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        var fileExtension = Path.GetExtension(originalFileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadDir, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await stream.CopyToAsync(fileStream, cancellationToken);
        }

        return $"/uploads/{subDirectory}/{uniqueFileName}".Replace("\\", "/");
    }
}
