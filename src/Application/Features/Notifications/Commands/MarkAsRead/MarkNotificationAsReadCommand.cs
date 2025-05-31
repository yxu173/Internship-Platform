using Application.Abstractions.Messaging;

namespace Application.Features.Notifications.Commands.MarkAsRead;

public record MarkNotificationAsReadCommand(Guid NotificationId) : ICommand;