using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.UpdateRoadmap;

public sealed class UpdateRoadmapCommandHandler : ICommandHandler<UpdateRoadmapCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public UpdateRoadmapCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(UpdateRoadmapCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.Id);
        if (roadmap == null)
            return Result.Failure<Guid>(RoadmapErrors.RoadmapNotFound);

        roadmap.UpdateRoadmap(request.Title, request.Description, request.Technology, request.IsPremium, request.Price);

        await _roadmapRepository.Update(roadmap);
        return Result.Success(roadmap.Id);
    }
}