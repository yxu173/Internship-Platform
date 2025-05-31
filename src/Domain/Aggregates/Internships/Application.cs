using Domain.Aggregates.Profiles;
using Domain.Aggregates.Users;
using Domain.Common;
using Domain.Enums;
using SharedKernel;

namespace Domain.Aggregates.Internships;

public sealed class Application : BaseEntity
{
    public Guid InternshipId { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public DateTime AppliedAt { get; }
    public ApplicationStatus Status { get; private set; }
    public string? ResumeUrl { get; }
    public DateTime? DecisionDate { get; private set; }
    public string? FeedbackNotes { get; private set; }
    public Internship Internship { get; private set; }
    public StudentProfile StudentProfile { get; private set; }
    private Application() { }

    private Application(Guid studentProfileId, string resumeUrl)
    {
        Id = Guid.NewGuid();
        StudentProfileId = studentProfileId;
        AppliedAt = DateTime.UtcNow;
        Status = ApplicationStatus.Pending;
        ResumeUrl = resumeUrl;
    }

    public static Result<Application> CreateApplication(Guid studentProfileId, string resumeUrl)
    {
        return Result.Success(new Application(studentProfileId, resumeUrl));
    }

    public Result UpdateStatus(ApplicationStatus newStatus, string? feedbackNotes = null)
    {
        Status = newStatus;
        DecisionDate = DateTime.UtcNow;
        
        if (feedbackNotes is not null)
        {
            FeedbackNotes = feedbackNotes;
        }
        
        return Result.Success();
    }
}
