using Application.Features.Roadmaps.Commands.CreateRoadmap;
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
}