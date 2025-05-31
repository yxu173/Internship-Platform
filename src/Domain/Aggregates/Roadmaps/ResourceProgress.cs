using Domain.Common;

namespace Domain.Aggregates.Roadmaps;

public sealed class ResourceProgress : BaseEntity
{
    private ResourceProgress()
    {
    }

    public Guid EnrollmentId { get; private set; }
    public Guid ItemId { get; private set; }
    public DateTime CompletedAt { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public bool IsVerified { get; private set; }
    public Enrollment Enrollment { get; private set; }

    public ResourceProgress(Guid enrollmentId, Guid itemId)
    {
        EnrollmentId = enrollmentId;
        ItemId = itemId;
        CompletedAt = DateTime.UtcNow;
    }

    public void Verify()
    {
        IsVerified = true;
        ModifiedAt = DateTime.UtcNow;
    }
}