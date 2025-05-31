using Application.Abstractions.Services;
using Application.Features.StudentProfile.Commands.CreateStudentExperience;
using Application.Features.StudentProfile.Commands.CreateStudentProfile;
using Application.Features.StudentProfile.Commands.CreateStudentProject;
using Application.Features.StudentProfile.Commands.RemoveStudentExperience;
using Application.Features.StudentProfile.Commands.RemoveStudentProject;
using Application.Features.StudentProfile.Commands.UpdateStudentBio;
using Application.Features.StudentProfile.Commands.UpdateStudentEducation;
using Application.Features.StudentProfile.Commands.UpdateStudentExperience;
using Application.Features.StudentProfile.Commands.UpdateStudentInfo;
using Application.Features.StudentProfile.Commands.UpdateStudentProfilePic;
using Application.Features.StudentProfile.Commands.UpdateStudentProject;
using Application.Features.StudentProfile.Commands.UpdateStudentResumeUrl;
using Application.Features.StudentProfile.Queries.GetAllStudentExperiences;
using Application.Features.StudentProfile.Queries.GetAllStudentProfile;
using Application.Features.StudentProfile.Queries.GetAllStudentProjects;
using Application.Features.StudentProfile.Queries.GetAllStudentSkills;
using Application.Features.StudentProfile.Queries.GetStudentBio;
using Application.Features.StudentProfile.Queries.GetStudentEducation;
using Application.Features.StudentProfile.Queries.GetStudentProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Profile;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[Authorize]
public class StudentController : BaseController
{
    private readonly IPhotoUploadService _photoUploadService;

    public StudentController(IPhotoUploadService photoUploadService)
    {
        _photoUploadService = photoUploadService;
    }

    [HttpGet("profiles/{id:guid}")]
    public async Task<IResult> GetCompleteStudentProfile([FromRoute] Guid id)
    {
        var query = new GetAllStudentProfileQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("profile-picture/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfilePicture([FromRoute] Guid id)
    {
        var query = new GetAllStudentProfileQuery(id);
        var result = await _mediator.Send(query);
        
        if (result.IsFailure)
            return NotFound();
            
        var profilePicUrl = result.Value.ProfilePictureUrl;
        
        if (string.IsNullOrEmpty(profilePicUrl))
            profilePicUrl = "/uploads/profile-pics/default-profile.png";
            
        var path = profilePicUrl;
        if (profilePicUrl.Contains("/uploads/"))
        {
            path = profilePicUrl[(profilePicUrl.IndexOf("/uploads/"))..]; 
        }
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
        
        if (!System.IO.File.Exists(filePath))
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile-pics", "default-profile.png");
        }
        
        var contentType = GetContentType(filePath);
        return PhysicalFile(filePath, contentType);
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
    
    [HttpPost("upload-profile-picture")]
    public async Task<IResult> UploadProfilePicture([FromForm] IFormFile file)
    {
        var result = await _photoUploadService.UploadProfilePhoto(file);
        if (result.IsSuccess)
        {
            var command = new UpdateStudentProfilePicCommand(UserId, result.Value);
            await _mediator.Send(command);
        }

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("upload-resume")]
    public async Task<IResult> UploadResume([FromForm] IFormFile file)
    {
        var result = await _photoUploadService.UploadResumeFile(file);
        if (result.IsSuccess)
        {
            var command = new UpdateStudentResumeUrlCommand(UserId, result.Value);
            await _mediator.Send(command);
        }

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("profiles")]
    public async Task<IResult> CreateStudentProfile([FromForm] string fullName,
        [FromForm] string university,
        [FromForm] string faculty,
        [FromForm] int graduationYear,
        [FromForm] int enrollmentYear,
        [FromForm] int age,
        [FromForm] string gender,
        [FromForm] string phoneNumber,
        [FromForm] string? bio,
        [FromForm] IFormFile? profilePicture)
    {
        string? profilePictureUrl = null;

        if (profilePicture != null && profilePicture.Length > 0)
        {
            var pictureResult = await _photoUploadService.UploadProfilePhoto(profilePicture);


            profilePictureUrl = pictureResult.Value;
        }

        var command = new CreateStudentProfileCommand(
            UserId,
            fullName,
            university,
            faculty,
            graduationYear,
            enrollmentYear,
            age,
            gender,
            phoneNumber,
            bio,
            profilePictureUrl);

        var result = await _mediator.Send(command);


        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/info")]
    public async Task<IResult> GetStudentProfile()
    {
        var query = new GetStudentProfileQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // [HttpGet("profiles/basic/{id:guid}")]
    // public async Task<IResult> GetStudentProfileById([FromRoute] Guid id)
    // {
    //     var query = new GetStudentProfileQuery(id);
    //     var result = await _mediator.Send(query);
    //     return result.Match(Results.Ok, CustomResults.Problem);
    // }

    [HttpPut("profiles/Info")]
    public async Task<IResult> UpdateStudentProfile([FromBody] UpdateStudentProfileRequest request)
    {
        var command = new UpdateStudentInfoCommand(
            UserId,
            request.FullName,
            request.PhoneNumber,
            request.Location,
            request.Age,
            request.Gender);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("profiles/Education")]
    public async Task<IResult> UpdateEducation([FromBody] UpdateEducationRequest request)
    {
        var command = new UpdateStudentEducationCommand(
            UserId,
            request.University,
            request.Faculty,
            request.GraduationYear,
            request.EnrollmentYear,
            request.Role);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/Education")]
    public async Task<IResult> UpdateEducation()
    {
        var query = new GetStudentEducationQuery(
            UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("profiles/bio")]
    public async Task<IResult> UpdateBio([FromBody] UpdateBioRequest request)
    {
        var command = new UpdateStudentBioCommand(UserId, request.Bio);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/bio")]
    public async Task<IResult> UpdateBio()
    {
        var query = new GetStudentBioQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }


    // Add Student ID to the route
    [HttpGet("profiles/skills")]
    public async Task<IResult> GetStudentSkills()
    {
        var query = new GetAllStudentSkillsQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("profiles/me/experience")]
    public async Task<IResult> AddExperience([FromBody] AddExperienceRequest request)
    {
        var command = new CreateStudentExperienceCommand(
            UserId,
            request.JobTitle,
            request.CompanyName,
            request.StartDate,
            request.EndDate);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("profiles/{experienceId:guid}/experience")]
    public async Task<IResult> UpdateExperience([FromRoute] Guid experienceId, AddExperienceRequest request)
    {
        var command = new UpdateStudentExperienceCommand(UserId,
            experienceId,
            request.JobTitle,
            request.CompanyName,
            request.StartDate,
            request.EndDate);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("profiles/me/experience/{id:guid}")]
    public async Task<IResult> RemoveExperience([FromRoute] Guid id)
    {
        var command = new RemoveStudentExperienceCommand(id);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/{id:guid}/experiences")]
    public async Task<IResult> GetStudentExperiences([FromRoute] Guid id)
    {
        var query = new GetAllStudentExperiencesQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("profiles/me/projects")]
    public async Task<IResult> AddProject([FromBody] AddProjectRequest request)
    {
        var command = new CreateStudentProjectCommand(
            UserId,
            request.ProjectName,
            request.Description,
            request.ProjectUrl);

        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/{id:guid}/projects")]
    public async Task<IResult> GetStudentProjects([FromRoute] Guid id)
    {
        var query = new GetAllStudentProjectsQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("profiles/{id:guid}/projects/{projectId:guid}")]
    public async Task<IResult> UpdateProject([FromRoute] Guid id, [FromRoute] Guid projectId,
        [FromBody] UpdateProjectRequest request)
    {
        var command = new UpdateStudentProjectCommand(
            id,
            projectId,
            request.ProjectName,
            request.Description,
            request.ProjectUrl);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("profiles/{id:guid}/projects/{projectId:guid}")]
    public async Task<IResult> RemoveProject([FromRoute] Guid id, [FromRoute] Guid projectId)
    {
        var command = new RemoveStudentProjectCommand(id, projectId);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}