using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.UpdateStudentProfilePic;

public sealed class UpdateStudentProfilePicCommandHandler : ICommandHandler<UpdateStudentProfilePicCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentProfilePicCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(UpdateStudentProfilePicCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.GetByIdAsync(request.UserId);
            
        result.UpdateProfilePicture(request.ProfilePicUrl);

        await _studentRepository.UpdateAsync(result);
        return Result.Success(true);
    }
}