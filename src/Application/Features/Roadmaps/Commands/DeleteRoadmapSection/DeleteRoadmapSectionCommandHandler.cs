using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
namespace Application.Features.Roadmaps.Commands.DeleteRoadmapSection;

internal sealed class DeleteRoadmapSectionCommandHandler : ICommandHandler<DeleteRoadmapSectionCommand>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public DeleteRoadmapSectionCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result> Handle(DeleteRoadmapSectionCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);

        if (roadmap is null)
        {
            return Result.Failure(RoadmapErrors.RoadmapNotFound);
        }

        // TODO: Add authorization check - does the user own this roadmap?
        var section = roadmap.Sections.First(x => x.Id == request.SectionId);
        if (section is null)
        {
            return Result.Failure(RoadmapErrors.SectionNotFound);
        }
        await _roadmapRepository.DeleteSectionAsync(section);
        var removeResult = roadmap.RemoveSection(request.SectionId);

        if (removeResult.IsFailure)
        {
            return removeResult; 
        }
        
        return Result.Success();
    }
} 