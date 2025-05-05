using Application.Abstractions.Messaging;
using Application.Features.Notifications.Commands.SendNotification;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Internships.SetApplicationUnderReview;

public sealed class SetApplicationUnderReviewCommandHandler : ICommandHandler<SetApplicationUnderReviewCommand, bool>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IMediator _mediator;

    public SetApplicationUnderReviewCommandHandler(IInternshipRepository internshipRepository, IMediator mediator)
    {
        _internshipRepository = internshipRepository;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(SetApplicationUnderReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var application = await _internshipRepository.GetApplicationByIdAsync(request.ApplicationId);

            if (application == null)
            {
                return Result.Failure<bool>(Error.NotFound("Application not found", $"{request.ApplicationId}"));
            }

            var internship = await _internshipRepository.GetByIdAsync(application.InternshipId);
            if (internship == null)
            {
                return Result.Failure<bool>(Error.NotFound("Internship not found", $"{application.InternshipId}"));
            }

            application.UpdateStatus(ApplicationStatus.UnderReview, request.FeedbackNotes);
            await _internshipRepository.UpdateApplicationAsync(application);

            var notificationTitle = "Application Under Review";
            var notificationMessage = $"Your application for {internship.Title} is now under review.";
            if (!string.IsNullOrEmpty(request.FeedbackNotes))
            {
                notificationMessage += $" Note: {request.FeedbackNotes}";
            }

            var notificationCommand = new SendNotificationCommand(
                application.StudentProfile.UserId,
                notificationTitle,
                notificationMessage,
                "Application",
                application.Id);

            await _mediator.Send(notificationCommand, cancellationToken);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>(Error.BadRequest("Error setting application under review", $"{ex.Message}"));
        }
    }
}
