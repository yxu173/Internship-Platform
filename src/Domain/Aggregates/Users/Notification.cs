using Domain.Common;
using SharedKernel;

namespace Domain.Aggregates.Users;

public sealed class Notification : BaseAuditableEntity
{
    private Notification(
        Guid userId,
        string title,
        string message,
        string entity,
        Guid entityId)
    {
        UserId = userId;
        Title = title;
        Message = message;
        Entity = entity;
        EntityId = entityId;
        IsRead = false;
    }

    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public string Entity { get; private set; }
    public Guid EntityId { get; private set; }
    public bool IsRead { get; private set; }

    public static Result<Notification> Create(
        Guid userId,
        string title,
        string message,
        string entity,
        Guid entityId)
    {
        var notification = new Notification(userId, title, message, entity, entityId);
        return Result.Success(notification);
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}