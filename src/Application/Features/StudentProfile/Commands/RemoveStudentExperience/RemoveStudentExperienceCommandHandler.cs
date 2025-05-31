using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.RemoveStudentExperience;

public sealed class RemoveStudentExperienceCommandHandler : ICommandHandler<RemoveStudentExperienceCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public RemoveStudentExperienceCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(RemoveStudentExperienceCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.RemoveStudentExperienceAsync(request.ExperienceId);
        
        return result.IsFailure ? Result.Failure<bool>(result.Error) : Result.Success(true);
    }
}