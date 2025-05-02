namespace Web.Api.Contracts.Roadmap;

public record UpdateRoadmapSectionRequest(
    string Title,
    int Order
); 