using Domain.Aggregates.Profiles;
using Domain.Aggregates.Internships;
using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.Aggregates.Bookmarks;

public sealed class InternshipBookmark : BaseAuditableEntity
{
    private InternshipBookmark()
    {
    }

    private InternshipBookmark(Guid studentId, Guid internshipId)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        InternshipId = internshipId;
        CreatedAt = DateTime.UtcNow;
    }
    
    public Guid StudentId { get; private set; }
    public StudentProfile? Student { get; private set; }
    public Guid InternshipId { get; private set; }
    public Internship? Internship { get; private set; }

    public static Result<InternshipBookmark> Create(Guid studentId, Guid internshipId)
    {
        if (studentId == Guid.Empty)
            return Result.Failure<InternshipBookmark>(BookmarkErrors.InvalidStudentId);

        if (internshipId == Guid.Empty)
            return Result.Failure<InternshipBookmark>(BookmarkErrors.InvalidInternshipId);

        return Result.Success(new InternshipBookmark(studentId, internshipId));
    }
}