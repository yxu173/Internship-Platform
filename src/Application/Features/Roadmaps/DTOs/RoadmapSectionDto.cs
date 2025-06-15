using Application.Features.Roadmaps.DTOs;
using Domain.ValueObjects; // Assuming RoadmapItemDto uses this

namespace Application.Features.Roadmaps.DTOs;

public record RoadmapSectionDto(
    Guid Id,
    string Title,
    int Order,
    Guid? QuizId,
    IReadOnlyList<RoadmapItemDto> Items
);