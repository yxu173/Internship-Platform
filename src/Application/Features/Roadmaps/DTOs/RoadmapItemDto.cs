using System.Security.AccessControl;

namespace Application.Features.Roadmaps.DTOs;

public record RoadmapItemDto(
    string Title,
    List<ResourceLinkDto> Resources,
    int Order
);