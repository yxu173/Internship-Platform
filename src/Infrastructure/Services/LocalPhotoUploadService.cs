using System.Web;
using Application.Abstractions.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace Infrastructure.Services;

public class LocalPhotoUploadService : IPhotoUploadService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalPhotoUploadService(
        IWebHostEnvironment env,
        IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<string>> UploadProfilePhoto(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Result.Failure<string>(Error.Validation("PhotoUpload.EmptyFile", "No file uploaded"));

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            return Result.Failure<string>(Error.Validation("PhotoUpload.InvalidType", "Invalid image type"));

        var path = _env.WebRootPath + @"\Properties\wwwroot\profile-photos";
        var uploadsFolder = Path.Combine(path);
        Directory.CreateDirectory(uploadsFolder);
        
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var request = _httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var relativePath = HttpUtility.UrlPathEncode($"uploads/profile-pics/{fileName}");
        
        return Result.Success(fileName);
    }

    public async Task<Result> DeletePhoto(string photoUrl)
    {
        if (string.IsNullOrEmpty(photoUrl)) 
            return Result.Success();

        try
        {
            var uri = new Uri(photoUrl);
            var localPath = uri.LocalPath.TrimStart('/');
            var fullPath = Path.Combine(_env.WebRootPath, localPath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
            
            return Result.Success();
        }
        catch
        {
            return Result.Failure(Error.Validation("PhotoUpload.DeleteFailed", "Failed to delete photo"));
        }
    }
}