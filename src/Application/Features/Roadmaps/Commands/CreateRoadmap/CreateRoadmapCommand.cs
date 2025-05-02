using Application.Abstractions.Messaging;

namespace Application.Features.Roadmaps.Commands.CreateRoadmap;

public sealed record CreateRoadmapCommand(
    Guid UserId,
    string Title,
    string Description,
    string Technology,
    bool IsPremium,
    decimal Price
) : ICommand<Guid>;