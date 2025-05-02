using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.CreateStudentProfile;

public sealed class CreateStudentProfileCommandHandler : ICommandHandler<CreateStudentProfileCommand, Guid>
{
    private readonly IStudentRepository _studentRepository;

    public CreateStudentProfileCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }


    public async Task<Result<Guid>> Handle(CreateStudentProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.CreateAsync(
            request.UserId,
            request.FullName,
            request.University,
            request.Faculty,
            request.GraduationYear,
            request.EnrollmentYear,
            request.Age,
            request.Gender,
            request.PhoneNumber,
            request.Bio,
            request.ProfilePictureUrl);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(UserErrors.UserNotFound);
        }

        return Result.Success(result.Value.Id);
    }
}