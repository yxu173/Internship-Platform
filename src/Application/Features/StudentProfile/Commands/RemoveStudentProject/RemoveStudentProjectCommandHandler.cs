using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.RemoveStudentProject;

public sealed class RemoveStudentProjectCommandHandler : ICommandHandler<RemoveStudentProjectCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public RemoveStudentProjectCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(RemoveStudentProjectCommand request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository
            .RemoveStudentProjectAsync(request.StudentId, request.ProjectId);

        return result.IsFailure ? Result.Failure<bool>(result.Error) : Result.Success(true);
    }
}