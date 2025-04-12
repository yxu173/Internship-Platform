using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.DeleteRoadmapItem;

public record DeleteRoadmapItemCommand(
    Guid RoadmapId,
    Guid SectionId,
    Guid ItemId
) : ICommand; 