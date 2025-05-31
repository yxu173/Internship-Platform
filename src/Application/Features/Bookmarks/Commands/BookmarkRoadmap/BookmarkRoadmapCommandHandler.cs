using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Aggregates.Bookmarks;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.BookmarkRoadmap;

internal sealed class BookmarkRoadmapCommandHandler : ICommandHandler<BookmarkRoadmapCommand,bool>
{
    private readonly IRoadmapBookmarkRepository _roadmapBookmarkRepository;
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IStudentRepository _studentRepository;

    public BookmarkRoadmapCommandHandler(
        IRoadmapBookmarkRepository roadmapBookmarkRepository,
        IRoadmapRepository roadmapRepository,
        IStudentRepository studentRepository)
    {
        _roadmapBookmarkRepository = roadmapBookmarkRepository;
        _roadmapRepository = roadmapRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(BookmarkRoadmapCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        if (student == null)
        {
            return Result.Failure<bool>(BookmarkErrors.InvalidStudentId);
        }

        var roadmapExists = await _roadmapRepository.GetByIdAsync(request.RoadmapId);
        if (roadmapExists == null)
        {
            return Result.Failure<bool>(BookmarkErrors.InvalidRoadmapId);
        }

        var existingBookmark = await _roadmapBookmarkRepository.FindByUserAndRoadmapIdAsync(
            student.Id, request.RoadmapId, cancellationToken);
        if (existingBookmark is not null)
        {
            return Result.Failure<bool>(BookmarkErrors.AlreadyBookmarked);
        }

        var bookmarkResult = RoadmapBookmark.Create(student.Id, request.RoadmapId);
        if (bookmarkResult.IsFailure)
        {
            return Result.Failure<bool>(bookmarkResult.Error);
        }

        await _roadmapBookmarkRepository.AddAsync(bookmarkResult.Value, cancellationToken);


        return Result.Success(true);
    }
}