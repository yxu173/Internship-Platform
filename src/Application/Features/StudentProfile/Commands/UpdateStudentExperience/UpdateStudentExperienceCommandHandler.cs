using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.UpdateStudentExperience;

public class UpdateStudentExperienceCommandHandler : ICommandHandler<UpdateStudentExperienceCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentExperienceCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(UpdateStudentExperienceCommand request, CancellationToken cancellationToken)
    {
        var studentExperience = await _studentRepository.GetStudentExperienceById(request.ExperienceId);
        

        var result = studentExperience.UpdateStudentExperience(
            request.JobTitle,
            request.CompanyName,
            request.StartDate,
            request.EndDate
        );

        await _studentRepository.UpdateStudentExperience(studentExperience);

        return result.IsFailure ? Result.Failure<bool>(result.Error) : Result.Success(true);
    }
}