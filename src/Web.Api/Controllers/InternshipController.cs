using Application.Features.Internships.CreateInternship;
using Application.Features.Internships.UpdateInternship;
using Application.Features.Profiles.CreateCompanyProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Internship;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;
[Authorize]
public class InternshipController : BaseController
{
    [HttpPost("CreateInternship")]
    public async Task<IResult> CreateInternship([FromBody] CreateInternshipRequest request)
    {
        var command = new CreateInternshipCommand(
        UserId,
        request.Title,
        request.Description,
        request.Type,
        request.StartDate,
        request.EndDate,
        request.ApplicationDeadline
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok,CustomResults.Problem);
    }

    [HttpPut("UpdateInternship/{id}")]
    public async Task<IResult> UpdateInternship([FromRoute] Guid id, [FromBody] UpdateInternshipRequest request)
    {
        var command = new UpdateInternshipCommand(
            id,
            request.Title,
            request.Description,
            request.ApplicationDeadline
        );

        var result = await _mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}