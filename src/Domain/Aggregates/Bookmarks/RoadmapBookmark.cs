using Domain.Aggregates.Profiles;
using Domain.Aggregates.Roadmaps;
using Domain.Common;
using Domain.DomainErrors;
using SharedKernel;

namespace Domain.Aggregates.Bookmarks;

public sealed class RoadmapBookmark : BaseAuditableEntity
{
    private RoadmapBookmark()
    {
    }

    private RoadmapBookmark(Guid studentId, Guid roadmapId)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        RoadmapId = roadmapId;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid StudentId { get; private set; }
    public StudentProfile? Student { get; private set; }
    public Guid RoadmapId { get; private set; }
    public Roadmap? Roadmap { get; private set; }

    public static Result<RoadmapBookmark> Create(Guid studentId, Guid roadmapId)
    {
        if (studentId == Guid.Empty)
            return Result.Failure<RoadmapBookmark>(BookmarkErrors.InvalidStudentId);

        if (roadmapId == Guid.Empty)
            return Result.Failure<RoadmapBookmark>(BookmarkErrors.InvalidRoadmapId);

        return Result.Success(new RoadmapBookmark(studentId, roadmapId));
    }
}