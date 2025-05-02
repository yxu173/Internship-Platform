using System.Collections.Generic;

namespace Application.Features.Roadmaps.DTOs;

public record RoadmapItemProgressDto(
    Guid Id, 
    string Title,
    string Description,
    string Type,
    List<ResourceLinkDto> Resources,
    int Order,
    bool IsCompleted
); 