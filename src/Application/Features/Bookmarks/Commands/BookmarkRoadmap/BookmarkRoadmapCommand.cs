using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.BookmarkRoadmap;

public sealed record BookmarkRoadmapCommand(
    Guid UserId,
    Guid RoadmapId) : ICommand<bool>; 