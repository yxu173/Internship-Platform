using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.CreateApplication;

public sealed class CreateApplicationCommandHandler : ICommandHandler<CreateApplicationCommand, bool>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IStudentRepository _studentRepository;

    public CreateApplicationCommandHandler(IInternshipRepository internshipRepository, IStudentRepository studentRepository)
    {
        _internshipRepository = internshipRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var studentProfile = await _studentRepository.GetByIdAsync(request.UserId);
        var internship = await _internshipRepository.GetById(request.InternshipId);
        
        var studentApplications = await _internshipRepository.GetApplicationsByStudentIdAsync(studentProfile.Id);

        if (studentApplications.Any(a => a.InternshipId == request.InternshipId))
        {
            return Result.Failure<bool>(Error.Validation("Duplicate Application", "Duplicate Application"));
        }
        
        if (internship == null)
            return Result.Failure<bool>(InternshipErrors.NotFound);
        
        var applicationResult = Domain.Aggregates.Internships.Application.CreateApplication(
            studentProfile.Id,
            request.ResumeUrl
        );
        if (applicationResult.IsFailure)
            return Result.Failure<bool>(applicationResult.Error);
        internship.Apply(applicationResult.Value);
        
        await _internshipRepository.AddApplicationAsync(applicationResult.Value);
        return Result.Success(true);
    }
}