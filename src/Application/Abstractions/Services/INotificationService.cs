using Application.Abstractions.Pagination; // Added for PagedList
using Application.Features.Notifications.Common; // Added for NotificationResponse
using SharedKernel; // Added for Result
using System; // Added for Guid
using System.Threading; // Added for CancellationToken
using System.Threading.Tasks; // Added for Task

namespace Application.Abstractions.Services;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string title, string message, string entity, Guid entityId);
    Task<Result<PagedList<NotificationResponse>>> GetAllNotificationsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}