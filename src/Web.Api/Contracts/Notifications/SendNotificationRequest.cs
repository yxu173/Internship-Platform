namespace Web.Api.Contracts.Notifications;

public record SendNotificationRequest(
    Guid UserId,
    string Title,
    string Message,
    string Entity,
    Guid EntityId);