using Application.Abstractions.Messaging;

namespace Application.Features.Internships.AcceptApplication;

public sealed record AcceptApplicationCommand(Guid ApplicationId, string? FeedbackNotes = null) : ICommand<bool>;