using Application.Features.Home.Queries.GetHomePageData;

namespace Application.Features.Home.Queries.Search;

public sealed record SearchResultsDto(
    IReadOnlyList<InternshipCardDto> Internships,
    IReadOnlyList<CompanyCardDto> Companies,
    IReadOnlyList<RoadmapCardDto> Roadmaps,
    int TotalInternships,
    int TotalCompanies,
    int TotalRoadmaps,
    int CurrentPage,
    int PageSize,
    int TotalPages,
    string SearchTerm,
    SearchType SearchType
);

public sealed record CompanyCardDto(
    Guid Id,
    string Name,
    string LogoUrl,
    string Industry,
    int ActiveInternships,
    string Location
);
