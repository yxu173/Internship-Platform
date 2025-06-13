using System.Collections.Generic;

namespace Application.Features.Roadmaps.DTOs;

public record RoadmapItemProgressDto(
    Guid Id, 
    string Title,
    List<ResourceLinkDto> Resources,
    int Order,
    bool IsCompleted
); 