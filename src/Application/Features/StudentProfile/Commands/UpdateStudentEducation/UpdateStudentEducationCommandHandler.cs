using Application.Abstractions.Messaging;
using Application.Features.StudentProfile.Commands.UpdateStudentInfo;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.UpdateStudentEducation;

public sealed class UpdateStudentEducationCommandHandler : ICommandHandler<UpdateStudentEducationCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentEducationCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(UpdateStudentEducationCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.GetByIdAsync(request.UserId);

        result.UpdateEducation(
            request.University,
            request.Faculty,
            request.GraduationYear,
            request.EnrollmentYear,
            request.Role
        );

        await _studentRepository.UpdateAsync(result);

        return Result.Success(true);
    }
}