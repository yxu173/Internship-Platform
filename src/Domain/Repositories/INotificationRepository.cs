using Domain.Aggregates.Users;

namespace Domain.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
    Task<Notification?> GetByIdAsync(Guid id);
}