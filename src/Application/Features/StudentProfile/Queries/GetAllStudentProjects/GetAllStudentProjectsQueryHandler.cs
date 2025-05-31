using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Queries.GetAllStudentProjects;

public class
    GetAllStudentProjectsQueryHandler : IQueryHandler<GetAllStudentProjectsQuery,
    IReadOnlyList<StudentProjectDto>>
{
    private readonly IStudentRepository _studentRepository;

    public GetAllStudentProjectsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<IReadOnlyList<StudentProjectDto>>> Handle(GetAllStudentProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var studentProjects = await _studentRepository.GetAllStudentProjects(request.StudentId);
        
        return studentProjects.Select(p => new StudentProjectDto(
                p.ProjectName,
                p.Description,
                p.ProjectUrl))
            .ToList();
    }
}