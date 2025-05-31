using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Aggregates.Bookmarks;
using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.BookmarkInternship;

internal sealed class BookmarkInternshipCommandHandler : ICommandHandler<BookmarkInternshipCommand,bool>
{
    private readonly IInternshipBookmarkRepository _internshipBookmarkRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IStudentRepository _studentRepository;

    public BookmarkInternshipCommandHandler(
        IInternshipBookmarkRepository internshipBookmarkRepository,
        IInternshipRepository internshipRepository,
        IStudentRepository studentRepository)
    {
        _internshipBookmarkRepository = internshipBookmarkRepository;
        _internshipRepository = internshipRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(BookmarkInternshipCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        if (student == null)
        {
            return Result.Failure<bool>(BookmarkErrors.InvalidStudentId);
        }


        var internshipExists = await _internshipRepository.GetById(request.InternshipId);
        if (internshipExists == null)
        {
            return Result.Failure<bool>(BookmarkErrors.InvalidInternshipId);
        }

        var existingBookmark = await _internshipBookmarkRepository.FindByStudentAndInternshipIdAsync(
            student.Id, request.InternshipId, cancellationToken);
        if (existingBookmark is not null)
        {
            return Result.Failure<bool>(BookmarkErrors.AlreadyBookmarked);
        }

        var bookmarkResult = InternshipBookmark.Create(student.Id, request.InternshipId);
        if (bookmarkResult.IsFailure)
        {
            return Result.Failure<bool>(bookmarkResult.Error);
        }

        await _internshipBookmarkRepository.AddAsync(bookmarkResult.Value, cancellationToken);


        return Result.Success(true);
    }
}