using Application.Abstractions.Messaging;

namespace Application.Features.Notifications.Commands.DeleteNotification;

public sealed record DeleteNotificationCommand(Guid NotificationId) : ICommand; 