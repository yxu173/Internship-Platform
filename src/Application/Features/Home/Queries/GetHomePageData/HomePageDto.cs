namespace Application.Features.Home.Queries.GetHomePageData;

public sealed record HomePageDto(
    IReadOnlyList<InternshipCardDto> RecentInternships,
    IReadOnlyList<InternshipCardDto> RecommendedInternships,
    IReadOnlyList<InternshipCardDto> Internships,
    IReadOnlyList<ProjectCardDto> RealWorldProjects,
    IReadOnlyList<RoadmapCardDto> Roadmaps
);
