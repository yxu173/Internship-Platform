using Domain.Aggregates.Profiles;
using Domain.Common;
using Domain.DomainErrors;
using Domain.ValueObjects;
using SharedKernel;

namespace Domain.Aggregates.Roadmaps;

public sealed class Enrollment : BaseAuditableEntity
{
    private Enrollment()
    {
    }

    private readonly List<ResourceProgress> _progress = new();

    public Guid StudentId { get; private set; }
    public StudentProfile? Student { get; set; }
    public Guid RoadmapId { get; private set; }
    public Roadmap? Roadmap { get; set; }
    public DateTime EnrolledAt { get; }
    public string? PaymentProviderTransactionId { get; private set; }
    public decimal? AmountPaid { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public IReadOnlyList<ResourceProgress> Progress => _progress.AsReadOnly();

    private Enrollment(Guid studentId, Guid roadmapId)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        RoadmapId = roadmapId;
        EnrolledAt = DateTime.UtcNow;
        PaymentStatus = PaymentStatus.Pending;
    }

    public static Result<Enrollment> Create(Guid studentId, Guid roadmapId)
    {
        if (studentId == Guid.Empty)
            return Result.Failure<Enrollment>(RoadmapErrors.InvalidStudentId);
        if (roadmapId == Guid.Empty)
            return Result.Failure<Enrollment>(RoadmapErrors.InvalidRoadmapId);
        return Result.Success(new Enrollment(studentId, roadmapId));
    }

    public Result MarkItemCompleted(Guid itemId, Roadmap roadmap)
    {
        if (!_progress.Any(p => p.ItemId == itemId) &&
            roadmap.Sections.Any(s => s.Items.Any(i => i.Id == itemId)))
        {
            _progress.Add(new ResourceProgress(Id, itemId));
            return Result.Success();
        }

        return Result.Failure(RoadmapErrors.InvalidOrDuplicateItem);
    }

    public Result CompletePayment(string paymentProviderTransactionId, decimal amountPaid)
    {
        if (PaymentStatus == PaymentStatus.Completed)
        {
            return Result.Success();
        }

        PaymentStatus = PaymentStatus.Completed;
        PaymentProviderTransactionId = paymentProviderTransactionId;
        AmountPaid = amountPaid;
        ModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public double CalculateCompletionPercentage(Roadmap roadmap)
    {
        var totalItems = roadmap.Sections.Sum(s => s.Items.Count);
        return totalItems > 0 ? (double)_progress.Count / totalItems * 100 : 0;
    }
}