using Application.Abstractions.Messaging;
using Application.Abstractions.Pagination;
using Application.Features.Notifications.Common;
using Domain.Aggregates.Users;
using Domain.Repositories;
using SharedKernel;
using System.Collections.Generic;
using System.Linq;

namespace Application.Features.Notifications.Queries.GetAllNotifications;

public sealed class GetAllNotificationsQueryHandler
    : IQueryHandler<GetAllNotificationsQuery, PagedList<NotificationResponse>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetAllNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<PagedList<NotificationResponse>>> Handle(
        GetAllNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notificationsEnumerable = await _notificationRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
        
        if (notificationsEnumerable is null || !notificationsEnumerable.Any())
        {
            var emptyPagedList = PagedList<NotificationResponse>.Create(
                new List<NotificationResponse>(), 
                request.PageNumber, 
                request.PageSize);
            return Result.Success(emptyPagedList);
        }

        var notificationResponses = notificationsEnumerable
            .Select(n => new NotificationResponse(
                n.Id,
                n.Title,
                n.Message,
                n.Entity,
                n.EntityId,
                n.IsRead,
                n.CreatedAt 
            ))
            .ToList(); 

        var pagedList = PagedList<NotificationResponse>.Create(
            notificationResponses,
            request.PageNumber,
            request.PageSize);

        return Result.Success(pagedList);
    }
}
