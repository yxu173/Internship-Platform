namespace Application.Abstractions.Services;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string title, string message, string entity, Guid entityId);
}