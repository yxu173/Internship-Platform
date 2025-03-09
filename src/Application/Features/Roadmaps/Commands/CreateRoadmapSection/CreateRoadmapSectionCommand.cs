using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.Commands.CreateRoadmapSection;

public sealed record CreateRoadmapSectionCommand(
    Guid RoadmapId,
    string Title,
    int Order,
    List<RoadmapItemDto> Items
) : ICommand<Guid>;