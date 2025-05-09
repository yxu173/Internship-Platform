using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.UpdateStudentBio;

public sealed class UpdateStudentBioCommandHandler : ICommandHandler<UpdateStudentBioCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentBioCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(UpdateStudentBioCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.GetByIdAsync(request.UserId);

        result.UpdateBio(
            request.Bio
        );

        await _studentRepository.UpdateAsync(result);

        return Result.Success(true);
    }
}