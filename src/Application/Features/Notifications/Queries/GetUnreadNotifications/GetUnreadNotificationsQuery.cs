using Application.Abstractions.Messaging;
using Application.Features.Notifications.Common;

namespace Application.Features.Notifications.Queries.GetUnreadNotifications;

public record GetUnreadNotificationsQuery(Guid UserId) : IQuery<IReadOnlyList<NotificationResponse>>;