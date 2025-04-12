using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.DeleteRoadmapSection;
 
public record DeleteRoadmapSectionCommand(Guid RoadmapId, Guid SectionId) : ICommand; 