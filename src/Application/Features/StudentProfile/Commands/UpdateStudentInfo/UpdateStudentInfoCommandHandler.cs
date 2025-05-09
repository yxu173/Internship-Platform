using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.UpdateStudentInfo;

public sealed class UpdateStudentInfoCommandHandler : ICommandHandler<UpdateStudentInfoCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentInfoCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(UpdateStudentInfoCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.GetByIdAsync(request.UserId);

        result.UpdateInformation(
            request.FullName,
            request.PhoneNumber,
            request.Location,
            request.Age,
            request.Gender);

        await _studentRepository.UpdateAsync(result);

        return Result.Success(true);
    }
}