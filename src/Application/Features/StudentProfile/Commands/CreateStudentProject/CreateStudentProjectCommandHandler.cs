using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Commands.CreateStudentProject;

public sealed class CreateStudentProjectCommandHandler : ICommandHandler<CreateStudentProjectCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public CreateStudentProjectCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(CreateStudentProjectCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.UserId);
        var result = await _studentRepository.CreateStudentProjectAsync(
            student.Id,
            request.ProjectName,
            request.Description,
            request.ProjectUrl);
        return result.IsFailure ? Result.Failure<bool>(result.Error) : Result.Success(true);
    }
}