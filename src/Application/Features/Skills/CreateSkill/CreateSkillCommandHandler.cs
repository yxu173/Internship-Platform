namespace Application.Features.Skills.CreateSkill
{
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Abstractions.Messaging;
    using Domain.Repositories;
    using SharedKernel;

    public sealed class CreateSkillCommandHandler : ICommandHandler<CreateSkillCommand, bool>
    {
        private readonly ISkillRepository _skillRepository;

        public CreateSkillCommandHandler(
            ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<Result<bool>> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
        {
            await _skillRepository.CreateAsync(request.Name);

            return Result.Success(true);
        }
    }
}