using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Queries.GetAllStudentSkills;

public sealed class GetAllStudentSkillsQueryHandler : IQueryHandler<GetAllStudentSkillsQuery, IReadOnlyList<Skill>>
{
    private readonly IStudentRepository _studentRepository;

    public GetAllStudentSkillsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<IReadOnlyList<Skill>>> Handle(GetAllStudentSkillsQuery request,
        CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.UserId);
        var studentSkills = await _studentRepository.GetStudentSkillsAsync(student.Id);
        var skills = studentSkills.Select(s => s.Skill).ToList();
        return Result.Success<IReadOnlyList<Skill>>(skills);
    }
}