using Application.Abstractions.Messaging;

namespace Application.Features.Internships.SetApplicationUnderReview;

public sealed record SetApplicationUnderReviewCommand(Guid ApplicationId, string? FeedbackNotes = null) : ICommand<bool>;
