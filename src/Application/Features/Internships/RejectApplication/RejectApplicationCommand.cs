using Application.Abstractions.Messaging;

namespace Application.Features.Internships.RejectApplication;

public sealed record RejectApplicationCommand(Guid ApplicationId, string? FeedbackNotes = null) : ICommand<bool>;