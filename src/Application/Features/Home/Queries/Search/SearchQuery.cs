using Application.Abstractions.Messaging;

namespace Application.Features.Home.Queries.Search;

public sealed record SearchQuery(
    string SearchTerm, 
    SearchType Type,
    Guid? UserId = null,
    int Page = 1, 
    int PageSize = 10) : IQuery<SearchResultsDto>;

public enum SearchType
{
    All,
    Internships,
    Companies,
    Roadmaps
}
