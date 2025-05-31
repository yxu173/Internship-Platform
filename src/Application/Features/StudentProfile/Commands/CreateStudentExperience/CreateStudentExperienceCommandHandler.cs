using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.CreateStudentExperience;

public class CreateStudentExperienceCommandHandler : ICommandHandler<CreateStudentExperienceCommand, Guid>
{
    private readonly IStudentRepository _studentRepository;

    public CreateStudentExperienceCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<Guid>> Handle(CreateStudentExperienceCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.UserId);
        var result = await _studentRepository.CreateStudentExperienceAsync(student.Id,
            request.JobTitle,
            request.CompanyName,
            request.StartDate,
            request.EndDate);
        
        return result.IsFailure ? Result.Failure<Guid>(result.Error) : Result.Success(student.Id);
    }
}