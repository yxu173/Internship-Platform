using Application.Abstractions.Messaging;
using Application.Features.Notifications.Commands.SendNotification;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Internships.AcceptApplication;

public class AcceptApplicationCommandHandler : ICommandHandler<AcceptApplicationCommand, bool>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IMediator _mediator;

    public AcceptApplicationCommandHandler(IInternshipRepository internshipRepository, IMediator mediator)
    {
        _internshipRepository = internshipRepository;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(AcceptApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var application = await _internshipRepository.GetApplicationByIdAsync(request.ApplicationId);

            if (application == null)
            {
                return Result.Failure<bool>(Error.NotFound("Application not found", $"{request.ApplicationId}"));
            }

            // Get the internship to include in notification message
            var internship = await _internshipRepository.GetByIdAsync(application.InternshipId);
            if (internship == null)
            {
                return Result.Failure<bool>(Error.NotFound("Internship not found", $"{application.InternshipId}"));
            }

            application.UpdateStatus(ApplicationStatus.Accepted, request.FeedbackNotes);
            await _internshipRepository.UpdateApplicationAsync(application);

            // Send notification to the student
            var notificationTitle = "Application Accepted!";
            var notificationMessage = $"Your application for {internship.Title} has been accepted.";
            if (!string.IsNullOrEmpty(request.FeedbackNotes))
            {
                notificationMessage += $" Feedback: {request.FeedbackNotes}";
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
            return Result.Failure<bool>(Error.BadRequest("Error accepting application", $"{ex.Message}"));
        }
    }
}