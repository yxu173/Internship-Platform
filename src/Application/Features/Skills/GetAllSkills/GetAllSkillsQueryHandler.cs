using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Skills.GetAllSkills;

public sealed class GetAllSkillsQueryHandler : IQueryHandler<GetAllSkillsQuery, IEnumerable<Skill>>
{
    private readonly ISkillRepository _skillRepository;

    public GetAllSkillsQueryHandler(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<Result<IEnumerable<Skill>>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
    {
        var skills = await _skillRepository.GetAllAsync();


        return Result.Success(skills);
    }
}