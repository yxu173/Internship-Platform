using Domain.ValueObjects;

namespace Web.Api.Contracts.Roadmap;

public record CreateRoadmapSectionItemRequest(
    Guid RoadmapSectionId, 
    string Title, 
    string Description,
    string Type, 
    List<ResourceLink> Resources,
    int Order);