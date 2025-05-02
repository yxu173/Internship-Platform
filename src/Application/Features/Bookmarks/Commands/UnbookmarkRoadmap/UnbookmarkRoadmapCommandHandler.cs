using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.UnbookmarkRoadmap;

internal sealed class UnbookmarkRoadmapCommandHandler : ICommandHandler<UnbookmarkRoadmapCommand, bool>
{
    private readonly IRoadmapBookmarkRepository _roadmapBookmarkRepository;

    public UnbookmarkRoadmapCommandHandler(
        IRoadmapBookmarkRepository roadmapBookmarkRepository)
    {
        _roadmapBookmarkRepository = roadmapBookmarkRepository;
    }

    public async Task<Result<bool>> Handle(UnbookmarkRoadmapCommand request, CancellationToken cancellationToken)
    {
        var bookmark = await _roadmapBookmarkRepository.FindByUserAndRoadmapIdAsync(
            request.StudentId, request.RoadmapId, cancellationToken);

        if (bookmark is null)
        {
            return Result.Failure<bool>(BookmarkErrors.BookmarkNotFound);
        }

        _roadmapBookmarkRepository.Remove(bookmark);


        return Result.Success(true);
    }
}