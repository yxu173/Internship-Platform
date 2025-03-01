using Domain.Common;
using Domain.DomainErrors;
using Domain.Enums;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class Enrollment : BaseEntity
{
    public Guid StudentId { get; }
    public DateTime EnrolledAt { get; }
    public PaymentStatus PaymentStatus { get; private set; }
    public Dictionary<Guid, DateTime> CompletedItems { get; } = new();

    public Result AccessContent(RoadmapItem item)
    {
        if (item.IsPremium && PaymentStatus != PaymentStatus.Paid)
            return Result.Failure(RoadmapErrors.PaidRoadmap);

        return Result.Success();
    }

    public Result MarkItemCompleted(Guid itemId)
    {
        CompletedItems[itemId] = DateTime.UtcNow;
        return Result.Success();
    }
}