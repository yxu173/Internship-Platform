using Application.Features.Roadmaps.Commands.CreateRoadmap;
using Application.Features.Roadmaps.Commands.CreateRoadmapSection;
using Application.Features.Roadmaps.Commands.UpdateRoadmap;
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

    [HttpPost("{roadmapId}/sections")]
    public async Task<IResult> CreateSection([FromRoute] Guid roadmapId, [FromBody] CreateRoadmapSectionCommand command)
    {
        command = command with { RoadmapId = roadmapId };
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}