namespace Application.Features.Home.Queries.GetHomePageData;

public sealed record RoadmapCardDto(
    Guid Id,
    string Title,
    string CompanyName,
    string CompanyLogoUrl,
    string Technology,
    string Category,
    int EnrollmentCount,
    bool IsBookmarked
);
