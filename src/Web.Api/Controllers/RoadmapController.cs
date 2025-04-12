using Application.Features.Roadmaps.Commands.CreateRoadmap;
using Application.Features.Roadmaps.Commands.CreateRoadmapSection;
using Application.Features.Roadmaps.Commands.UpdateRoadmap;
using Application.Features.Roadmaps.Commands.DeleteRoadmap;
using Application.Features.Roadmaps.Commands.UpdateRoadmapSection;
using Application.Features.Roadmaps.Commands.DeleteRoadmapSection;
using Application.Features.Roadmaps.Commands.AddRoadmapItem;
using Application.Features.Roadmaps.Commands.UpdateRoadmapItem;
using Application.Features.Roadmaps.Commands.DeleteRoadmapItem;
using Application.Features.Roadmaps.Commands.EnrollStudent;
using Application.Features.Roadmaps.Commands.TrackProgress;
using Application.Features.Roadmaps.Queries.GetRoadmapById;
using Application.Features.Roadmaps.Queries.GetRoadmapsByTechnology;
using Application.Features.Roadmaps.Queries.GetRoadmapsByCompanyId;
using Application.Features.Roadmaps.Queries.GetPublicRoadmaps;
using Application.Features.Roadmaps.Queries.GetStudentEnrollments;
using Application.Features.Roadmaps.Queries.GetStudentRoadmapProgress;
using Application.Features.Roadmaps.Queries.GetCompanyRoadmapEnrollments;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Roadmap;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

public class RoadmapController : BaseController
{
    [HttpPost("create")]
    public async Task<IResult> Create([FromBody] CreateRoadmapRequest request)
    {
        var command = new CreateRoadmapCommand(
            UserId,
            request.Title,
            request.Description,
            request.Technology,
            request.IsPremium,
            request.Price);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<IResult> Update([FromRoute] Guid id, [FromBody] UpdateRoadmapRequest request)
    {
        var command = new UpdateRoadmapCommand(
            id,
            request.Title,
            request.Description,
            request.Technology,
            request.IsPremium,
            request.Price);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> Delete([FromRoute] Guid id)
    {
        // TODO: Add authorization check - ensure user owns the roadmap
        var command = new DeleteRoadmapCommand(id);
        var result = await _mediator.Send(command);
        return result.Match(() => Results.NoContent(), CustomResults.Problem);
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetById([FromRoute] Guid id, [FromQuery] bool includeSections = false)
    {
        var query = new GetRoadmapByIdQuery(id, includeSections);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("by-technology/{technology}")]
    public async Task<IResult> GetByTechnology([FromRoute] string technology)
    {
        var query = new GetRoadmapsByTechnologyQuery(technology);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("by-company/{companyId:guid}")]
    public async Task<IResult> GetByCompanyId([FromRoute] Guid companyId)
    {
        var query = new GetRoadmapsByCompanyIdQuery(companyId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("public")]
    public async Task<IResult> GetPublic([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetPublicRoadmapsQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("{roadmapId}/sections")]
    public async Task<IResult> CreateSection([FromRoute] Guid roadmapId, [FromBody] CreateRoadmapSectionRequest request)
    {
        var command = new CreateRoadmapSectionCommand(roadmapId, request.Title, request.Order);
        var result = await _mediator.Send(command);
        return result.Match(sectionId => Results.Created($"/api/roadmap/{roadmapId}/sections/{sectionId}", sectionId),
            CustomResults.Problem);
    }

    [HttpPut("{roadmapId}/sections/{sectionId}")]
    public async Task<IResult> UpdateSection(
        [FromRoute] Guid roadmapId,
        [FromRoute] Guid sectionId,
        [FromBody] UpdateRoadmapSectionRequest request)
    {
        var command = new UpdateRoadmapSectionCommand(
            roadmapId,
            sectionId,
            request.Title,
            request.Order
        );
        var result = await _mediator.Send(command);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    [HttpDelete("{roadmapId}/sections/{sectionId}")]
    public async Task<IResult> DeleteSection([FromRoute] Guid roadmapId, [FromRoute] Guid sectionId)
    {
        // TODO: Add authorization check
        var command = new DeleteRoadmapSectionCommand(roadmapId, sectionId);
        var result = await _mediator.Send(command);
        return result.Match(() => Results.NoContent(), CustomResults.Problem);
    }


    [HttpPost("{roadmapId}/sections/{sectionId}/items")]
    public async Task<IResult> AddItem(
        [FromRoute] Guid roadmapId,
        [FromRoute] Guid sectionId,
        [FromBody] CreateRoadmapItemRequest request)
    {
        var resourceDtos = request.Resources
            .Select(r => new Application.Features.Roadmaps.DTOs.ResourceLinkDto(r.Title, r.Url, r.Type))
            .ToList();

        var command = new AddRoadmapItemCommand(
            roadmapId,
            sectionId,
            request.Title,
            request.Description,
            request.Type,
            resourceDtos,
            request.Order
        );

        var result = await _mediator.Send(command);

        return result.Match(
            itemId => Results.Created($"/api/roadmaps/{roadmapId}/sections/{sectionId}/items/{itemId}", itemId),
            CustomResults.Problem
        );
    }

    [HttpPut("{roadmapId}/sections/{sectionId}/items/{itemId}")]
    public async Task<IResult> UpdateItem(
        [FromRoute] Guid roadmapId,
        [FromRoute] Guid sectionId,
        [FromRoute] Guid itemId,
        [FromBody] UpdateRoadmapItemRequest request)
    {
        var resourceDtos = request.Resources
            .Select(r => new Application.Features.Roadmaps.DTOs.ResourceLinkDto(r.Title, r.Url, r.Type))
            .ToList();

        var command = new UpdateRoadmapItemCommand(
            roadmapId,
            sectionId,
            itemId,
            request.Title,
            request.Description,
            request.Type,
            resourceDtos,
            request.Order
        );

        var result = await _mediator.Send(command);

        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    [HttpDelete("{roadmapId}/sections/{sectionId}/items/{itemId}")]
    public async Task<IResult> DeleteItem(
        [FromRoute] Guid roadmapId,
        [FromRoute] Guid sectionId,
        [FromRoute] Guid itemId)
    {
        var command = new DeleteRoadmapItemCommand(
            roadmapId,
            sectionId,
            itemId
        );

        var result = await _mediator.Send(command);

        return result.Match(() => Results.NoContent(), CustomResults.Problem);
    }

    [HttpPost("{roadmapId}/enroll")]
    public async Task<IResult> Enroll([FromRoute] Guid roadmapId)
    {
        var command = new EnrollStudentCommand(UserId, roadmapId);
        var result = await _mediator.Send(command);

        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    [HttpPost("{roadmapId}/items/{itemId}/track-progress")]
    public async Task<IResult> TrackProgress([FromRoute] Guid roadmapId, [FromRoute] Guid itemId)
    {
        var command = new TrackProgressCommand(UserId, roadmapId, itemId);
        var result = await _mediator.Send(command);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    [HttpGet("my-enrollments")]
    public async Task<IResult> GetMyEnrollments()
    {
        var query = new GetStudentEnrollmentsQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // [HttpGet("company/enrollments")]
    // public async Task<IResult> GetCompanyEnrollments([FromQuery] Guid? roadmapId = null)
    // {
    //     // TODO: Get CompanyId based on authenticated user (assuming user is company rep)
    //     Guid companyId = GetCompanyIdForCurrentUser();
    //     var query = new GetCompanyRoadmapEnrollmentsQuery(companyId, roadmapId);
    //     var result = await _mediator.Send(query);
    //     return result.Match(Results.Ok, CustomResults.Problem);
    // }
    //
}