namespace Application.Features.Home.Queries.GetHomePageData;

public sealed record ProjectCardDto(
    Guid Id,
    string Title,
    string CompanyName,
    string CompanyLogoUrl,
    string ProjectType,
    string Technologies,
    int ApplicationCount,
    bool IsBookmarked
);
