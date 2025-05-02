namespace Application.Features.Skills.CreateSkill
{
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Abstractions.Messaging;
    using Domain.Repositories;
    using SharedKernel;

    public sealed class CreateSkillCommandHandler : ICommandHandler<CreateSkillCommand, Guid>
    {
        private readonly ISkillRepository _skillRepository;

        public CreateSkillCommandHandler(
            ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<Result<Guid>> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
        {
            var skill = await _skillRepository.CreateAsync(request.Name);

            return Result.Success(skill.Value.Id);
        }
    }
}