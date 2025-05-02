using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.DeleteRoadmap;

internal sealed class DeleteRoadmapCommandHandler : ICommandHandler<DeleteRoadmapCommand>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public DeleteRoadmapCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result> Handle(DeleteRoadmapCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId);

        if (roadmap is null)
        {
            return Result.Failure(RoadmapErrors.RoadmapNotFound);
        }

        // TODO: Add authorization check - does the user own this roadmap?

        await _roadmapRepository.Delete(roadmap);

        return Result.Success();
    }
} 