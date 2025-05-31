using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace Application.Abstractions.Services;

public interface IPhotoUploadService
{
    Task<Result<string>> UploadProfilePhoto(IFormFile file);
    Task<Result<string>> UploadResumeFile(IFormFile file);
    Task<Result<string>> UploadCompanyLogo(IFormFile file);
    Task<Result> DeletePhoto(string photoUrl);
}