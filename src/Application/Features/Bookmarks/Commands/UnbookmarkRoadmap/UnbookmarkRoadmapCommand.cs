using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.UnbookmarkRoadmap;

public sealed record UnbookmarkRoadmapCommand(
    Guid UserId,
    Guid RoadmapId) : ICommand<bool>; 