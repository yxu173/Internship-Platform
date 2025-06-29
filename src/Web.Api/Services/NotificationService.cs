using Application.Abstractions.Services;
using Application.Abstractions.Pagination;
using Application.Features.Notifications.Common;
using Application.Features.Notifications.Queries.GetAllNotifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SharedKernel;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Hubs;

namespace Web.Api.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ISender _sender;

    public NotificationService(IHubContext<NotificationHub> hubContext, ISender sender)
    {
        _hubContext = hubContext;
        _sender = sender;
    }

    public async Task SendNotificationAsync(Guid userId, string title, string message, string entity, Guid entityId)
    {
        var notification = new
        {
            Title = title,
            Message = message,
            Entity = entity,
            EntityId = entityId,
            Timestamp = DateTime.UtcNow
        };  

        await _hubContext.Clients.Group(userId.ToString())
            .SendAsync("ReceiveNotification", notification);
    }

    public async Task<Result<PagedList<NotificationResponse>>> GetAllNotificationsAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetAllNotificationsQuery(userId, pageNumber, pageSize);
        return await _sender.Send(query, cancellationToken);
    }
}