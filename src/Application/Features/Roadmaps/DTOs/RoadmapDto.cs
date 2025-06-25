using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.DTOs;

public record RoadmapDto(
    Guid Id,
    string Title,
    string Description,
    string Technology,
    bool IsPremium,
    decimal? Price,
    Guid CompanyId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<RoadmapSectionDto>? Sections,
    bool IsEnrolled,
    RoadmapSectionDto? FirstSectionWithQuiz
);