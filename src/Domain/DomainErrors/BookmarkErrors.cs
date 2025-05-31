using SharedKernel;

namespace Domain.DomainErrors;

public static class BookmarkErrors
{
    public static readonly Error InvalidStudentId =
        Error.Validation("Bookmark.InvalidStudentId", "StudentId cannot be empty");

    public static readonly Error InvalidInternshipId =
        Error.Validation("Bookmark.InvalidInternshipId", "InternshipId cannot be empty");

    public static readonly Error InvalidRoadmapId =
        Error.Validation("Bookmark.InvalidRoadmapId", "RoadmapId cannot be empty");

    public static readonly Error DuplicateBookmark =
        Error.Validation("Bookmark.Duplicate", "This item is already bookmarked");

    public static readonly Error BookmarkNotFound =
        Error.NotFound("Bookmark.NotFound", "Bookmark not found");

    public static readonly Error AlreadyBookmarked =
        Error.Conflict("Bookmark.AlreadyBookmarked", "This item is already bookmarked");
}