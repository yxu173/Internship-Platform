using Application.Abstractions.Messaging;
using Application.Features.Notifications.Common;

namespace Application.Features.Notifications.Commands.SendNotification;

public record SendNotificationCommand(
    Guid UserId,
    string Title,
    string Message,
    string Entity,
    Guid EntityId
) : ICommand<NotificationResponse>;