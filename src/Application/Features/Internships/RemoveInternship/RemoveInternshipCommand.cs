using Application.Abstractions.Messaging;

namespace Application.Features.Internships.RemoveInternship;

public sealed record RemoveInternshipCommand(Guid InternshipId) : ICommand<bool>;
