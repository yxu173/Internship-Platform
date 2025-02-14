using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.CreateStudentExperience;

public class CreateStudentExperienceCommandHandler : ICommandHandler<CreateStudentExperienceCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public CreateStudentExperienceCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(CreateStudentExperienceCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.UserId);
        var result = await _studentRepository.CreateStudentExperienceAsync(student.Id,
            request.JobTitle,
            request.CompanyName,
            request.StartDate,
            request.EndDate);
        
        return result.IsFailure ? Result.Failure<bool>(result.Error) : Result.Success(true);
    }
}