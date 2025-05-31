using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.UpdateStudentProject;

public sealed class UpdateStudentProjectCommandHandler : ICommandHandler<UpdateStudentProjectCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentProjectCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(UpdateStudentProjectCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository.UpdateStudentProjectAsync(request.StudentId,
            request.ProjectId,
            request.ProjectName,
            request.Description,
            request.ProjectUrl);
        return result.IsFailure ? Result.Failure<bool>(result.Error) : Result.Success(true);
    }
}