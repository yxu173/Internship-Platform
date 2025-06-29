using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.UnbookmarkRoadmap;

internal sealed class UnbookmarkRoadmapCommandHandler : ICommandHandler<UnbookmarkRoadmapCommand, bool>
{
    private readonly IRoadmapBookmarkRepository _roadmapBookmarkRepository;
    private readonly IStudentRepository _studentRepository;

    public UnbookmarkRoadmapCommandHandler(
        IRoadmapBookmarkRepository roadmapBookmarkRepository, IStudentRepository studentRepository)
    {
        _roadmapBookmarkRepository = roadmapBookmarkRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(UnbookmarkRoadmapCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        var bookmark = await _roadmapBookmarkRepository.FindByUserAndRoadmapIdAsync(
            student.Id, request.RoadmapId, cancellationToken);

        if (bookmark is null)
        {
            return Result.Failure<bool>(BookmarkErrors.BookmarkNotFound);
        }

        _roadmapBookmarkRepository.Remove(bookmark);


        return Result.Success(true);
    }
}