using Domain.Aggregates.Users;
using System.Collections.Generic; // Added for IEnumerable
using System.Threading; // Added for CancellationToken

namespace Domain.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(Guid userId);
    Task<IEnumerable<Notification>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken); // New method
    Task MarkAsReadAsync(Guid notificationId);
    Task<Notification?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid notificationId);
}