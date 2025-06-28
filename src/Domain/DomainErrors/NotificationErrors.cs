using SharedKernel;

namespace Domain.DomainErrors;

public static class NotificationErrors
{
    public static readonly Error InvalidUserId =
        Error.Validation("Notification.InvalidUserId", "UserId cannot be empty");

    public static readonly Error InvalidTitle =
        Error.Validation("Notification.InvalidTitle", "Title cannot be empty");

    public static readonly Error NotFound =
        Error.NotFound("Notification.NotFound", "Notification not found");
}