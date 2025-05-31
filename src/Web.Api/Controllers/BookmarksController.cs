using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Bookmarks.Commands.BookmarkInternship;
using Application.Features.Bookmarks.Commands.UnbookmarkInternship;
using Application.Features.Bookmarks.Commands.BookmarkRoadmap;
using Application.Features.Bookmarks.Commands.UnbookmarkRoadmap;
using Application.Features.Bookmarks.Queries.GetBookmarkedItems;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[Authorize]
public class BookmarksController : BaseController
{
    [HttpGet]
    public async Task<IResult> GetBookmarkedItems(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetBookmarkedItemsQuery(UserId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("internships/{internshipId:guid}")]
    public async Task<IResult> BookmarkInternship([FromRoute] Guid internshipId)
    {
        var command = new BookmarkInternshipCommand(UserId, internshipId);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("internships/{internshipId:guid}")]
    public async Task<IResult> UnbookmarkInternship([FromRoute] Guid internshipId)
    {
        var command = new UnbookmarkInternshipCommand(UserId, internshipId);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("roadmaps/{roadmapId:guid}")]
    public async Task<IResult> BookmarkRoadmap([FromRoute] Guid roadmapId)
    {
        var command = new BookmarkRoadmapCommand(UserId, roadmapId);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpDelete("roadmaps/{roadmapId:guid}")]
    public async Task<IResult> UnbookmarkRoadmap([FromRoute] Guid roadmapId)
    {
        var command = new UnbookmarkRoadmapCommand(UserId, roadmapId);
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }
}