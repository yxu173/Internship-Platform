using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps;
using System.Linq;

namespace Application.Features.Roadmaps.Commands.DeleteRoadmapItem;

internal sealed class DeleteRoadmapItemCommandHandler : ICommandHandler<DeleteRoadmapItemCommand>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public DeleteRoadmapItemCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result> Handle(DeleteRoadmapItemCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);
        if (roadmap is null)
        {
            return Result.Failure(RoadmapErrors.RoadmapNotFound);
        }

        // TODO: Add authorization check

        var section = roadmap.Sections.FirstOrDefault(s => s.Id == request.SectionId);
        if (section is null)
        {
            return Result.Failure(RoadmapErrors.SectionNotFound);
        }

        var removeResult = section.RemoveItem(request.ItemId);

        if (removeResult.IsFailure)
        {
            return removeResult;
        }

        await _roadmapRepository.Update(roadmap);

        return Result.Success();
    }
} 