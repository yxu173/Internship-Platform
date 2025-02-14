using Application.Features.StudentProfile.Commands.CreateStudentExperience;
using Application.Features.StudentProfile.Commands.CreateStudentProfile;
using Application.Features.StudentProfile.Commands.CreateStudentProject;
using Application.Features.StudentProfile.Commands.RemoveStudentExperience;
using Application.Features.StudentProfile.Commands.UpdateStudentInfo;
using Application.Features.StudentProfile.Commands.UpdateStudentProfilePic;
using Application.Features.StudentProfile.Commands.UpdateStudentResumeUrl;
using Application.Features.StudentProfile.Queries.GetAllStudentExperiences;
using Application.Features.StudentProfile.Queries.GetAllStudentSkills;
using Application.Features.StudentProfile.Queries.GetStudentProfile;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Profile;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

public class StudentController : BaseController
{
    [HttpPost("profiles")]
    public async Task<IResult> CreateStudentProfile([FromBody] CreateStudentProfileRequest request)
    {
        var command = new CreateStudentProfileCommand(
            UserId,
            request.FullName,
            request.University,
            request.Faculty,
            request.GraduationYear,
            request.EnrollmentYear,
            request.Age,
            request.Gender,
            request.PhoneNumber);

        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/me")]
    public async Task<IResult> GetStudentProfile()
    {
        var query = new GetStudentProfileQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("profiles/{id:guid}")]
    public async Task<IResult> GetStudentProfileById([FromRoute] Guid id)
    {
        var query = new GetStudentProfileQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("profiles/me")]
    public async Task<IResult> UpdateStudentProfile([FromBody] UpdateStudentProfileRequest request)
    {
        var command = new UpdateStudentInfoCommand(
            UserId,
            request.FullName,
            request.University,
            request.Faculty,
            request.GraduationYear,
            request.EnrollmentYear,
            request.Age,
            request.Bio,
            request.Gender);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/me/picture")]
    public async Task<IResult> UpdateStudentProfilePicture([FromBody] string profilePicture)
    {
        var command = new UpdateStudentProfilePicCommand(UserId, profilePicture);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("profiles/me/resume")]
    public async Task<IResult> UpdateStudentResume([FromBody] string resume)
    {
        var command = new UpdateStudentResumeUrlCommand(UserId, resume);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

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
}