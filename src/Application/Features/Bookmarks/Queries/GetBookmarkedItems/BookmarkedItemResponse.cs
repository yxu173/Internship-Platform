namespace Application.Features.Bookmarks.Queries.GetBookmarkedItems;

public enum BookmarkType
{
    Internship,
    Roadmap
}

public sealed record BookmarkedItemResponse(
    Guid Id,
    Guid BookmarkId,
    BookmarkType Type,
    string Title,
    string CompanyName,
    string? CompanyLogoUrl,
    List<string> Tags,
    bool IsActive,
    string Status,
    DateTime CreatedAt
);