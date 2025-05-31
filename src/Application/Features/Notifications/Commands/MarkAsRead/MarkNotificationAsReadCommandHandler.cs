using Application.Abstractions.Messaging;
using Application.Features.Notifications.Commands.MarkAsRead;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Notifications.Commands.SendNotification;

public sealed class MarkNotificationAsReadCommandHandler : ICommandHandler<MarkNotificationAsReadCommand>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);
        if (notification is null)
        {
            return Result.Failure(Error.NotFound("Notification not found",
                "The notification you are trying to mark as read does not exist."));
        }

        await _notificationRepository.MarkAsReadAsync(request.NotificationId);
        return Result.Success();
    }
}