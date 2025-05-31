using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Features.Notifications.Commands.SendNotification;
using Application.Features.Notifications.Common;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Notifications.Queries.GetUnreadNotifications;

public sealed class GetUnreadNotificationsQueryHandler
    : IQueryHandler<GetUnreadNotificationsQuery, IReadOnlyList<NotificationResponse>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUnreadNotificationsQueryHandler(
        INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<IReadOnlyList<NotificationResponse>>> Handle(
        GetUnreadNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == null)
        {
            return Result.Failure<IReadOnlyList<NotificationResponse>>(Error.Problem("User not authenticated",
                "You must be authenticated to get your notifications."));
        }

        var notifications = await _notificationRepository
            .GetUnreadNotificationsAsync(request.UserId);

        var response = notifications.Select(n => new NotificationResponse(
            n.Id,
            n.Title,
            n.Message,
            n.Entity,
            n.EntityId,
            n.IsRead,
            n.CreatedAt
        )).ToList();

        return Result.Success<IReadOnlyList<NotificationResponse>>(response);
    }
}