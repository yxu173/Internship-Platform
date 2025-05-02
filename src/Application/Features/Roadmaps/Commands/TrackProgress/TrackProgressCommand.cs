using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.TrackProgress;

public record TrackProgressCommand(
    Guid UserId, 
    Guid RoadmapId, 
    Guid ItemId     
) : ICommand;