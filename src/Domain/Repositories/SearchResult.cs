namespace Domain.Repositories;

public sealed class SearchResult<T> where T : class
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int CurrentPage { get; }
    public int PageSize { get; }
    public int TotalPages { get; }

    public SearchResult(IReadOnlyList<T> items, int totalCount, int currentPage, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }
}
