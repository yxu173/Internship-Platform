using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.Features.Notifications.Common;
using Domain.Aggregates.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Notifications.Commands.SendNotification;

public sealed class SendNotificationCommandHandler : ICommandHandler<SendNotificationCommand, NotificationResponse>
{
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;

    public SendNotificationCommandHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository)
    {
        _notificationService = notificationService;
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<NotificationResponse>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        // Create the notification entity
        var notification = Notification.Create(
            request.UserId,
            request.Title,
            request.Message,
            request.Entity,
            request.EntityId);

        if (notification.IsFailure)
        {
            return Result.Failure<NotificationResponse>(notification.Error);
        }

        // Save to database
        await _notificationRepository.AddAsync(notification.Value);

        // Send real-time notification
        await _notificationService.SendNotificationAsync(
            request.UserId,
            request.Title,
            request.Message,
            request.Entity,
            request.EntityId);

        var response = new NotificationResponse(
            notification.Value.Id,
            notification.Value.Title,
            notification.Value.Message,
            notification.Value.Entity,
            notification.Value.EntityId,
            notification.Value.IsRead,
            notification.Value.CreatedAt);

        return Result.Success(response);
    }
}