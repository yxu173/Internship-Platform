using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.UnbookmarkInternship;

internal sealed class UnbookmarkInternshipCommandHandler : ICommandHandler<UnbookmarkInternshipCommand, bool>
{
    private readonly IInternshipBookmarkRepository _internshipBookmarkRepository;

    public UnbookmarkInternshipCommandHandler(
        IInternshipBookmarkRepository internshipBookmarkRepository)
    {
        _internshipBookmarkRepository = internshipBookmarkRepository;
    }

    public async Task<Result<bool>> Handle(UnbookmarkInternshipCommand request, CancellationToken cancellationToken)
    {
        var bookmark = await _internshipBookmarkRepository.FindByUserAndInternshipIdAsync(
            request.UserId, request.InternshipId, cancellationToken);

        if (bookmark is null)
        {
            return Result.Failure<bool>(BookmarkErrors.BookmarkNotFound); 
        }

        await _internshipBookmarkRepository.Remove(bookmark);


        return Result.Success(true);
    }
} 