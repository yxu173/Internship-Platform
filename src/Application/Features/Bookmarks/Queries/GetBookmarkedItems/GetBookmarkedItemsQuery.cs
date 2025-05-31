using Application.Abstractions.Messaging;
using Application.Abstractions.Pagination;

namespace Application.Features.Bookmarks.Queries.GetBookmarkedItems;

public sealed record GetBookmarkedItemsQuery(
    Guid UserId,
    int PageNumber,
    int PageSize) : IQuery<PagedList<BookmarkedItemResponse>>; 