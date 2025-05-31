using Application.Abstractions.Messaging;

namespace Application.Features.Roadmaps.Commands.UpdateRoadmap;

public sealed record UpdateRoadmapCommand(
    Guid Id,
    string Title,
    string Description,
    string Technology,
    bool IsPremium,
    decimal? Price)
    : ICommand<Guid>;