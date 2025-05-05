using Domain.Enums;

namespace Application.Features.Internships.GetApplicationsByInternshipId;

public sealed record ApplicationDto(
    Guid Id,
    Guid StudentProfileId,
    string StudentName,
    string StudentTitle,
    string Education,
    DateTime AppliedAt,
    string? ResumeUrl,
    string Status,
    DateTime? DecisionDate,
    string? FeedbackNotes);