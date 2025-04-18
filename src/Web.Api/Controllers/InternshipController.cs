using Application.Features.Internships.CreateApplication;
using Application.Features.Internships.CreateInternship;
using Application.Features.Internships.GetInternshipsByCompanyId;
using Application.Features.Internships.RemoveApplication;
using Application.Features.Internships.RemoveInternship;
using Application.Features.Internships.UpdateInternship;
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
            request.About,
            request.KeyResponsibilities,
            request.Requirements,
            request.Type,
            request.WorkingModel,
            request.Salary,
            request.Currency,
            request.StartDate,
            request.EndDate,
            request.ApplicationDeadline
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("UpdateInternship/{id}")]
    public async Task<IResult> UpdateInternship([FromRoute] Guid id, [FromBody] UpdateInternshipRequest request)
    {
        var command = new UpdateInternshipCommand(
            id,
            request.Title,
            request.About,
            request.KeyResponsibilities,
            request.Requirements,
            request.Type,
            request.WorkingModel,
            request.Salary,
            request.Currency,
            request.ApplicationDeadline
        );

        var result = await _mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("DeleteInternship/{id}")]
    public async Task<IResult> DeleteInternship([FromRoute] Guid id)
    {
        var command = new RemoveInternshipCommand(id);

        var result = await _mediator.Send(command);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("CreateApplication/{id}")]
    public async Task<IResult> CreateApplication([FromRoute] Guid id, [FromBody] CreateApplicationRequest request)
    {
        var command = new CreateApplicationCommand(
            id,
            UserId,
            request.ResumeUrl
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("DeleteApplication/{id}")]
    public async Task<IResult> DeleteApplication([FromRoute] Guid id)
    {
        var command = new RemoveApplicationCommand(
            id,
            UserId
        );
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("GetInternshipById/{id:guid}")]
    public async Task<IResult> GetInternshipById([FromRoute] Guid id)
    {
        var query = new GetInternshipsByCompanyIdQuery(id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}