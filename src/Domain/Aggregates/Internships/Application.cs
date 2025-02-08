using Domain.Common;
using Domain.Enums;
using SharedKernel;

namespace Domain.Aggregates.Internships;

public sealed class Application : BaseEntity
{
    public Guid StudentId { get; }
    public DateTime AppliedAt { get; }
    public ApplicationStatus Status { get; private set; }
    public string? ResumeUrl { get; }
    public DateTime? DecisionDate { get; private set; }

    private Application() { }

    private Application(Guid studentId, string resumeUrl)
    {
        StudentId = studentId;
        AppliedAt = DateTime.UtcNow;
        Status = ApplicationStatus.Pending;
        ResumeUrl = resumeUrl;
    }

    public static Result<Application> CreateApplication(Guid studentId, string resumeUrl)
    {
        return Result.Success(new Application(studentId, resumeUrl));
    }

    public Result UpdateStatus(ApplicationStatus newStatus)
    {
        Status = newStatus;
        DecisionDate = DateTime.UtcNow;
        return Result.Success();
    }
}