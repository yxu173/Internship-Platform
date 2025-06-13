using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetPremiumRoadmaps;

public sealed class GetPremiumRoadmapsQueryHandler: IQueryHandler<GetPremiumRoadmapsQuery, IReadOnlyList<PublicRoadmapsDto>>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetPremiumRoadmapsQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<IReadOnlyList<PublicRoadmapsDto>>> Handle(GetPremiumRoadmapsQuery request, CancellationToken cancellationToken)
    {
        var roadmaps = await _roadmapRepository.GetPremiumRoadmapsAsync();

        var roadmapDtos = roadmaps.Select(roadmap => new PublicRoadmapsDto(
            roadmap.Id,
            roadmap.Title
        )).ToList();

        return Result.Success<IReadOnlyList<PublicRoadmapsDto>>(roadmapDtos);
    }
}