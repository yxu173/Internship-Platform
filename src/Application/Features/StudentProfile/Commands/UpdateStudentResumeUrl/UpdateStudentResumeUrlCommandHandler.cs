using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.UpdateStudentResumeUrl;

public sealed class UpdateStudentResumeUrlCommandHandler : ICommandHandler<UpdateStudentResumeUrlCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentResumeUrlCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(UpdateStudentResumeUrlCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.GetByIdAsync(request.UserId);

        result.UpdateResumeUrl(request.ResumeUrl);
        await _studentRepository.UpdateAsync(result);
        return Result.Success(true);
    }
}