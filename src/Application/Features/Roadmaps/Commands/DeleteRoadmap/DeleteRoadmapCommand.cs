using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.DeleteRoadmap;
 
public record DeleteRoadmapCommand(Guid RoadmapId) : ICommand; 