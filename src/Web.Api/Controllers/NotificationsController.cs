using Application.Features.Notifications.Commands.MarkAsRead;
using Application.Features.Notifications.Commands.SendNotification;
using Application.Features.Notifications.Queries.GetUnreadNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Contracts.Notifications;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : BaseController
{
    [HttpGet]
    public async Task<IResult> GetUnreadNotifications()
    {
        var query = new GetUnreadNotificationsQuery(UserId);
        var result = await _mediator.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost]
    public async Task<IResult> SendNotification([FromBody] SendNotificationRequest request)
    {
        var command = new SendNotificationCommand(
            request.UserId,
            request.Title,
            request.Message,
            request.Entity,
            request.EntityId);
            
        var result = await _mediator.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IResult> MarkAsRead(Guid id)
    {
        var command = new MarkNotificationAsReadCommand(id);
        var result = await _mediator.Send(command);
        return result.Match(() => Results.NoContent(), CustomResults.Problem);
    }
}