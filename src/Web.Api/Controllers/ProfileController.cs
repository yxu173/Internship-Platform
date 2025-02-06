using Application.Abstractions.Authentication;
using Application.Features.Profiles.CreateCompanyProfile;
using Application.Features.Profiles.StudentProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Profile;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;
[Authorize]
public class ProfileController : BaseController
{
    [HttpPost("student")]
    public async Task<IResult> CreateStudentProfile([FromBody] CreateStudentProfileRequest request)
    {

        var command = new CreateStudentProfileCommand(
            UserId,
            request.FullName,
            request.University,
            request.Faculty,
            request.GraduationYear,
            request.Age,
            request.Gender,
            request.PhoneNumber);

        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("company")]
    public async Task<IResult> CreateCompanyProfile([FromBody] CreateCompanyProfileRequest request)
    {
        var command = new CreateCompanyProfileCommand(
            UserId,
            request.CompanyName,
            request.TaxId,
            request.Governorate,
            request.Industry);

        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("student")]
    public async Task<IResult> GetStudentProfile()
    {
        var query = new GetStudentProfileQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}