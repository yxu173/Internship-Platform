using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.RemoveApplication;

internal sealed class RemoveApplicationCommandHandler
    : ICommandHandler<RemoveApplicationCommand, bool>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IStudentRepository _studentRepository;

    public RemoveApplicationCommandHandler(IInternshipRepository internshipRepository, IStudentRepository studentRepository)
    {
        _internshipRepository = internshipRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(
        RemoveApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var studentProfile = await _studentRepository.GetByIdAsync(request.UserId);
        var application = await _internshipRepository.GetApplicationByIdAsync(request.ApplicationId);
        if (application == null)
        {
            return Result.Failure<bool>(InternshipErrors.ApplicationNotFound);
        }

        if (application.StudentProfileId != studentProfile.Id)
        {
            return Result.Failure<bool>(InternshipErrors.NotApplicationOwner);
        }

        await _internshipRepository.RemoveApplication(application);
        return Result.Success(true);
    }
}