using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Profiles.StudentProfile;



public sealed class CreateStudentProfileCommandHandler : ICommandHandler<CreateStudentProfileCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public CreateStudentProfileCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }



    public async Task<Result<bool>> Handle(CreateStudentProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.CreateAsync(request.UserId,
         request.FullName,
          request.University,
           request.Faculty,
            request.GraduationYear,
             request.Age,
              request.Gender,
              request.PhoneNumber);

        if (result.IsFailure)
        {
            return Result.Failure<bool>(UserErrors.UserNotFound);
        }

        return Result.Success(true);
    }
}