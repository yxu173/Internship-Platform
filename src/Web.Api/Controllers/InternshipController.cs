using Application.Features.Internships.AcceptApplication;
using Application.Features.Internships.CreateApplication;
using Application.Features.Internships.CreateInternship;
using Application.Features.Internships.GetApplicationsByInternshipId;
using Application.Features.Internships.GetByInternshipId;
using Application.Features.Internships.GetInternshipsByCompanyId;
using Application.Features.Internships.RejectApplication;
using Application.Features.Internships.RemoveApplication;
using Application.Features.Internships.RemoveInternship;
using Application.Features.Internships.SetApplicationUnderReview;
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
    
    [HttpGet("GetInternship/{id:guid}")]
    public async Task<IResult> GetInternships([FromRoute] Guid id)
    {
        var query = new GetByInternshipIdCommand(UserId,id);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
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

    [HttpGet("GetInternshipById")]
    public async Task<IResult> GetInternshipById()
    {
        var query = new GetInternshipsByCompanyIdQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpGet("GetApplicationsByInternshipId/{internshipId:guid}")]
    public async Task<IResult> GetApplicationsByInternshipId([FromRoute] Guid internshipId)
    {
        var query = new GetApplicationsByInternshipIdQuery(internshipId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost("AcceptApplication/{applicationId:guid}")]
    public async Task<IResult> AcceptApplication([FromRoute] Guid applicationId, [FromBody] ApplicationFeedbackRequest request)
    {
        var command = new AcceptApplicationCommand(applicationId, request.FeedbackNotes);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost("RejectApplication/{applicationId:guid}")]
    public async Task<IResult> RejectApplication([FromRoute] Guid applicationId, [FromBody] ApplicationFeedbackRequest request)
    {
        var command = new RejectApplicationCommand(applicationId, request.FeedbackNotes);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
    
    [HttpPost("SetApplicationUnderReview/{applicationId:guid}")]
    public async Task<IResult> SetApplicationUnderReview([FromRoute] Guid applicationId, [FromBody] ApplicationFeedbackRequest request)
    {
        var command = new SetApplicationUnderReviewCommand(applicationId, request.FeedbackNotes);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}