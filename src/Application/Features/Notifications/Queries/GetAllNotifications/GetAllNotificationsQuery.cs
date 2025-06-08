using Application.Abstractions.Messaging;
using Application.Abstractions.Pagination;
using Application.Features.Notifications.Common;
using SharedKernel;

namespace Application.Features.Notifications.Queries.GetAllNotifications;

public record GetAllNotificationsQuery(
    Guid UserId,
    int PageNumber,
    int PageSize) : IQuery<PagedList<NotificationResponse>>;
