namespace Web.Api.Contracts.Roadmap;

public record CreateRoadmapRequest(
    string Title,
    string Description,
    string Technology,
    bool IsPremium,
    decimal Price);