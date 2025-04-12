namespace Web.Api.Contracts.Roadmap;

public record CreateRoadmapSectionRequest(
    string Title,
    int Order
);