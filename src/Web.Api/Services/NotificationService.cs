using Application.Abstractions.Services;
using Microsoft.AspNetCore.SignalR;
using Web.Api.Hubs;

namespace Web.Api.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
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
}