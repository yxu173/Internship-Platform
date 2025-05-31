using Application.Abstractions.Services;
using Application.Features.CompanyDashboard.Queries.GetCompanyDashboard;
using Application.Features.CompanyProfile.Commands.CreateCompanyProfile;
using Application.Features.CompanyProfile.Commands.UpdateCompanyAbout;
using Application.Features.CompanyProfile.Commands.UpdateCompanyAddress;
using Application.Features.CompanyProfile.Commands.UpdateCompanyBasicInfo;
using Application.Features.CompanyProfile.Commands.UpdateCompanyInfo;
using Application.Features.CompanyProfile.Commands.UpdateCompanyLogo;
using Application.Features.CompanyProfile.Queries.GetCompanyAbout;
using Application.Features.CompanyProfile.Queries.GetCompanyBasicInfo;
using Application.Features.CompanyProfile.Queries.GetCompanyContact;
using Application.Features.CompanyProfile.Queries.GetCompanyInfo;
using Application.Features.CompanyProfile.Queries.GetCompanyLogo;
using Application.Features.CompanyProfile.Queries.GetCompanyProfile;
using Application.Features.CompanyProfile.Queries.GetCompleteCompanyProfile;
using Application.Features.CompanyProfile.Queries.GetCompanyPosts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.CompanyProfile;
using Web.Api.Contracts.Profile;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[Authorize]
public class CompanyController : BaseController
{
    private readonly IPhotoUploadService _photoUploadService;

    public CompanyController(IPhotoUploadService photoUploadService)
    {
        _photoUploadService = photoUploadService;
    }

    [HttpGet("profiles/{id:guid}")]
    public async Task<IResult> GetCompleteCompanyProfile([FromRoute] Guid id)
    {
        var query = new GetCompleteCompanyProfileQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("company-logo/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompanyLogo([FromRoute] Guid id)
    {
        var query = new GetCompleteCompanyProfileQuery(id);
        var result = await _mediator.Send(query);
        
        if (result.IsFailure)
            return NotFound();
            
        var logoUrl = result.Value.Logo;
        
        if (string.IsNullOrEmpty(logoUrl))
            logoUrl = "/uploads/company-logos/default-logo.png";
            
        var path = logoUrl;
        if (logoUrl.Contains("/uploads/"))
        {
            path = logoUrl[(logoUrl.IndexOf("/uploads/"))..]; 
        }
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
        
        if (!System.IO.File.Exists(filePath))
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "company-logos", "default-logo.png");
        }
        
        var contentType = GetContentType(filePath);
        return PhysicalFile(filePath, contentType);
    }


    [HttpPost("profiles")]
    public async Task<IResult> CreateCompanyProfile(
        [FromForm] string companyName,
        [FromForm] string taxId,
        [FromForm] string governorate,
        [FromForm] string city,
        [FromForm] string industry,
        [FromForm] IFormFile? logo)
    {
        string? logoUrl = null;

        if (logo != null && logo.Length > 0)
        {
            var logoResult = await _photoUploadService.UploadCompanyLogo(logo);


            logoUrl = logoResult.Value;
        }

        var command = new CreateCompanyProfileCommand(
            UserId,
            companyName,
            taxId,
            governorate,
            city,
            industry);

        var result = await _mediator.Send(command);

        if (result.IsSuccess && logoUrl != null)
        {
            await _mediator.Send(new UpdateCompanyLogoCommand(UserId, logoUrl));
        }

        return result.Match(Results.Ok, CustomResults.Problem);
    }


    // [HttpGet("profiles")]
    // public async Task<IResult> GetCompanyProfile()
    // {
    //     var query = new GetCompanyProfileQuery(UserId);
    //     var result = await _mediator.Send(query);
    //     return result.Match(Results.Ok, CustomResults.Problem);
    // }

    /*
    [HttpGet("profiles/{id:guid}")]
    public async Task<IResult> GetCompanyProfileById(Guid id)
    {
        var query = new GetCompanyProfileQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }*/

    [HttpPatch("profiles/basic")]
    public async Task<IResult> UpdateBasicInfo([FromBody] UpdateCompanyBasicInfoRequest request)
    {
        var command = new UpdateCompanyBasicInfoCommand(
            UserId,
            request.Industry,
            request.WebsiteUrl,
            request.CompanySize,
            request.yearOfEstablishment);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/basic")]
    public async Task<IResult> GetCompanyBasicInfo()
    {
        var query = new GetCompanyBasicInfoQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/info")]
    public async Task<IResult> UpdateInfo([FromBody] UpdateCompanyInfoRequest request)
    {
        var command = new UpdateCompanyInfoCommand(
            UserId,
            request.Name,
            request.Description);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/info")]
    public async Task<IResult> GetInfo()
    {
        var query = new GetCompanyInfoQuery(
            UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/about")]
    public async Task<IResult> GetCompanyAbout()
    {
        var query = new GetCompanyAboutQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/about")]
    public async Task<IResult> UpdateCompanyAbout([FromBody] UpdateCompanyAboutRequest request)
    {
        var command = new UpdateCompanyAboutCommand(
            UserId,
            request.About,
            request.Mission,
            request.Vision
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/address")]
    public async Task<IResult> UpdateCompanyAddress([FromBody] UpdateAddressRequest request)
    {
        var command = new UpdateCompanyAddressCommand(UserId, request.Governorate, request.City);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("dashboard")]
    public async Task<IResult> GetCompanyDashboard()
    {
        var query = new GetCompanyDashboardQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("profiles/contact")]
    public async Task<IResult> GetCompanyContact()
    {
        var query = new GetCompanyContactQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream" // Default content type if extension is not recognized
        };
    }
    
    [HttpPost("upload-logo")]
    public async Task<IResult> UploadCompanyLogo([FromForm] IFormFile file)
    {
        var result = await _photoUploadService.UploadCompanyLogo(file);
        if (result.IsSuccess)
        {
            var command = new UpdateCompanyLogoCommand(UserId, result.Value);
            await _mediator.Send(command);
        }

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/logo")]
    public async Task<IResult> GetCompanyLogo()
    {
        var query = new GetCompanyLogoQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("posts")]
    public async Task<IResult> GetCompanyPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var query = new GetCompanyPostsQuery(UserId, page, pageSize);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}