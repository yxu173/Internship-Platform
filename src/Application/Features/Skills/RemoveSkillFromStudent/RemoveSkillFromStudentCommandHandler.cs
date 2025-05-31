using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Skills;

public sealed class RemoveSkillFromStudentCommandHandler 
    : ICommandHandler<RemoveSkillFromStudentCommand, bool>
{
    private readonly IStudentRepository _studentRepository;

    public RemoveSkillFromStudentCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<bool>> Handle(
        RemoveSkillFromStudentCommand request, 
        CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        if (student is null)
            return Result.Failure<bool>(Domain.DomainErrors.StudentErrors.ProfileNotFound);

        var skillToRemove = student.Skills
            .FirstOrDefault(ss => ss.SkillId == request.SkillId);
            
        if (skillToRemove is null)
            return Result.Failure<bool>(Domain.DomainErrors.StudentErrors.SkillNotFound);

        student.RemoveSkill(skillToRemove);
        await _studentRepository.UpdateAsync(student);
        return Result.Success(true);
    }
}
