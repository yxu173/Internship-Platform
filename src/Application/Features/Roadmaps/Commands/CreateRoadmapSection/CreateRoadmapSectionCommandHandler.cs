using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;


namespace Application.Features.Roadmaps.Commands.CreateRoadmapSection;

public sealed class CreateRoadmapSectionCommandHandler : ICommandHandler<CreateRoadmapSectionCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public CreateRoadmapSectionCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(CreateRoadmapSectionCommand request, CancellationToken cancellationToken)
    {
        var roadmap =
            await _roadmapRepository.GetByIdAsync(request.RoadmapId,
                includeSections: true); 
        if (roadmap == null)
        {
            return Result.Failure<Guid>(RoadmapErrors.RoadmapNotFound);
        }

        // TODO: Authorization check
        var addSectionResult = roadmap.AddSection(request.Title, request.Order);

        if (addSectionResult.IsFailure)
        {
            return Result.Failure<Guid>(addSectionResult.Error);
        }

        await _roadmapRepository.Update(roadmap);

        return addSectionResult;
    }
}
