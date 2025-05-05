using Application.Abstractions.Messaging;
using Domain.Enums;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.AcceptApplication;

public class AcceptApplicationCommandHandler : ICommandHandler<AcceptApplicationCommand, bool>
{
    private readonly IInternshipRepository _internshipRepository;

    public AcceptApplicationCommandHandler(IInternshipRepository internshipRepository)
    {
        _internshipRepository = internshipRepository;
    }

    public async Task<Result<bool>> Handle(AcceptApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var application = await _internshipRepository.GetApplicationByIdAsync(request.ApplicationId);

            if (application == null)
            {
                Result.Failure<bool>(Error.NotFound("Application not found", "${request.ApplicationId}"));
            }

            application.UpdateStatus(ApplicationStatus.Accepted);
            await _internshipRepository.UpdateApplicationAsync(application);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>(Error.BadRequest("Error accepting application", "${ex.Message}"));
        }
    }
}