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

       
        // Handle case where WebRootPath might be null
        string uploadRoot = _env.WebRootPath;
        if (string.IsNullOrEmpty(uploadRoot))
        {
            uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(uploadRoot);
        }

        var uploadsFolder = Path.Combine(uploadRoot, "uploads", "profile-pics");
        Directory.CreateDirectory(uploadsFolder);
    
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        // Generate full accessible URL
        var request = _httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var photoUrl = $"{baseUrl}/uploads/profile-pics/{fileName}";
    
        return Result.Success(photoUrl);
    }
    
    public async Task<Result<string>> UploadResumeFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Result.Failure<string>(Error.Validation("ResumeUpload.EmptyFile", "No file uploaded"));

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            return Result.Failure<string>(Error.Validation("ResumeUpload.InvalidType", "Invalid resume file type. Only PDF and Word documents are allowed."));

        // Handle case where WebRootPath might be null
        string uploadRoot = _env.WebRootPath;
        if (string.IsNullOrEmpty(uploadRoot))
        {
            uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(uploadRoot);
        }

        var uploadsFolder = Path.Combine(uploadRoot, "uploads", "resumes");
        Directory.CreateDirectory(uploadsFolder);
    
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        // Generate full accessible URL
        var request = _httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var resumeUrl = $"{baseUrl}/uploads/resumes/{fileName}";
    
        return Result.Success(resumeUrl);
    }
    
    public async Task<Result<string>> UploadCompanyLogo(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Result.Failure<string>(Error.Validation("LogoUpload.EmptyFile", "No file uploaded"));

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            return Result.Failure<string>(Error.Validation("LogoUpload.InvalidType", "Invalid logo image type. Only JPG, PNG, and SVG are allowed."));

        // Handle case where WebRootPath might be null
        string uploadRoot = _env.WebRootPath;
        if (string.IsNullOrEmpty(uploadRoot))
        {
            uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(uploadRoot);
        }

        var uploadsFolder = Path.Combine(uploadRoot, "uploads", "company-logos");
        Directory.CreateDirectory(uploadsFolder);
    
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        // Generate full accessible URL
        var request = _httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var logoUrl = $"{baseUrl}/uploads/company-logos/{fileName}";
    
        return Result.Success(logoUrl);
    }

    public async Task<Result> DeletePhoto(string photoUrl)
    {
        if (string.IsNullOrEmpty(photoUrl)) 
            return Result.Success();

        try
        {
            var uri = new Uri(photoUrl);
            var localPath = uri.LocalPath.TrimStart('/');
            // Handle case where WebRootPath might be null
            string uploadRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(uploadRoot))
            {
                uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            
            var fullPath = Path.Combine(uploadRoot, localPath);

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
