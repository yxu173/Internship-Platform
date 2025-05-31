using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps; // Needed for Roadmap

namespace Application.Features.Roadmaps.Commands.UpdateRoadmapSection;

internal sealed class UpdateRoadmapSectionCommandHandler : ICommandHandler<UpdateRoadmapSectionCommand>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public UpdateRoadmapSectionCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result> Handle(UpdateRoadmapSectionCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);

        if (roadmap is null)
        {
            return Result.Failure(RoadmapErrors.RoadmapNotFound);
        }

        // TODO: Add authorization check - does the user own this roadmap?

       var updateResult = roadmap.UpdateSection(request.SectionId, request.Title, request.Order);

        if (updateResult.IsFailure)
        {
            return updateResult; 
        }

        await _roadmapRepository.Update(roadmap);

        return Result.Success();
    }
} 