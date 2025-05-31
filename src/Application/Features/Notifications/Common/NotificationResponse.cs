namespace Application.Features.Notifications.Common;

public record NotificationResponse(
    Guid Id,
    string Title,
    string Message,
    string Entity,
    Guid EntityId,
    bool IsRead,
    DateTime CreatedOnUtc
);