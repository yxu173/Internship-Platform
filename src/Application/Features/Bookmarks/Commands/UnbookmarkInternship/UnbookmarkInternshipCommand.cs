using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Bookmarks.Commands.UnbookmarkInternship;

public sealed record UnbookmarkInternshipCommand(
    Guid UserId,
    Guid InternshipId) : ICommand<bool>; 