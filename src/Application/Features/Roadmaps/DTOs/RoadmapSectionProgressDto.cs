using System.Collections.Generic;

namespace Application.Features.Roadmaps.DTOs;

public record RoadmapSectionProgressDto(
    Guid Id,
    string Title,
    int Order,
    IReadOnlyList<RoadmapItemProgressDto> Items 
); 