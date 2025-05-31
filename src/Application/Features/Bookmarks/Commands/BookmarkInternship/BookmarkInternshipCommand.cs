using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.BookmarkInternship;

public sealed record BookmarkInternshipCommand(
    Guid UserId,
    Guid InternshipId) : ICommand<bool>; 