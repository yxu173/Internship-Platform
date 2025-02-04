using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Skills.DeleteSkill;

public sealed class DeleteSkillCommandHandler : ICommandHandler<DeleteSkillCommand, bool>
{
    private readonly ISkillRepository _skillRepository;

    public DeleteSkillCommandHandler(ISkillRepository skillRepository) =>
        _skillRepository = skillRepository;

    public async Task<Result<bool>> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var result =  await _skillRepository.DeleteAsync(request.SkillId);

        if (result.IsFailure)
        {
            return Result.Failure<bool>(SkillErrors.DeleteFailed);
        }
        
        return result.Value;

    }
}