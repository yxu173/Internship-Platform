using Application.Abstractions.Messaging;
using Domain.Aggregates.Roadmaps;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.CreateRoadmap;

public sealed class CreateRoadmapCommandHandler : ICommandHandler<CreateRoadmapCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public CreateRoadmapCommandHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<Guid>> Handle(CreateRoadmapCommand request, CancellationToken cancellationToken)
    {
        var roadmap = Roadmap.Create(request.Title
            , request.Description,
            request.Technology,
            request.IsPremium,
            request.Price,
            request.CompanyProfileId);
        
        await _roadmapRepository.AddAsync(roadmap.Value);
        
        return Result.Success(roadmap.Value.Id);
    }
}