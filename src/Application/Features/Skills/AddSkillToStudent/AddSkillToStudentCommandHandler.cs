using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Skills;

public sealed class AddSkillToStudentCommandHandler
    : ICommandHandler<AddSkillToStudentCommand, Guid>
{
    private readonly IStudentRepository _studentRepository;
    private readonly ISkillRepository _skillRepository;

    public AddSkillToStudentCommandHandler(
        IStudentRepository studentRepository,
        ISkillRepository skillRepository)
    {
        _studentRepository = studentRepository;
        _skillRepository = skillRepository;
    }

    public async Task<Result<Guid>> Handle(
        AddSkillToStudentCommand request,
        CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        if (student is null)
            return Result.Failure<Guid>(Domain.DomainErrors.StudentErrors.ProfileNotFound);

        var skill = await _skillRepository.GetByIdAsync(request.SkillId);
        if (skill is null)
            return Result.Failure<Guid>(Domain.DomainErrors.SkillErrors.NotFound);

        var result = student.AddSkill(skill);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        await _studentRepository.UpdateAsync(student);
        return Result.Success(skill.Id);
    }
}