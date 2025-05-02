namespace Web.Api.Contracts.Roadmap;

public sealed record UpdateRoadmapRequest(
    string Title,
    string Description,
    string Technology,
    bool IsPremium,
    decimal? Price);