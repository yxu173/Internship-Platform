using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.UpdateRoadmapSection;

public record UpdateRoadmapSectionCommand(
    Guid RoadmapId, 
    Guid SectionId,
    string Title,
    int Order
    ) : ICommand; 