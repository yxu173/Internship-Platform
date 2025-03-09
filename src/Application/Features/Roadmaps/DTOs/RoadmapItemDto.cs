using System.Security.AccessControl;

namespace Application.Features.Roadmaps.DTOs;

public record RoadmapItemDto(
    string Title,
    string Description,
    string Type,
    List<ResourceLinkDto> Resources,
    int Order
);