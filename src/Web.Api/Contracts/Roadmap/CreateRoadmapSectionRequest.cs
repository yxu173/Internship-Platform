namespace Web.Api.Contracts.Roadmap;

public sealed record CreateRoadmapSectionRequest(
    Guid RoadmapId, 
    string Title, 
    int Order,
    IEnumerable<CreateRoadmapSectionItemRequest> Items);