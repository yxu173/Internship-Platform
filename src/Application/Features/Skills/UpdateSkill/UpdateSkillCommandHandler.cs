using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Skills.UpdateSkill;

public class UpdateSkillCommandHandler : ICommandHandler<UpdateSkillCommand, Skill>
{
    private readonly ISkillRepository _skillRepository;

    public UpdateSkillCommandHandler(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<Result<Skill>> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        var result = await _skillRepository.UpdateAsync(request.Id);

        return Result.Success(result.Value);

    }
}