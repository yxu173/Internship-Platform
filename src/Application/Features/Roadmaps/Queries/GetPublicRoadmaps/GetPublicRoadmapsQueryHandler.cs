using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using SharedKernel;
using Domain.Aggregates.Roadmaps;
using System.Linq;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Queries.GetPublicRoadmaps;

internal sealed class GetPublicRoadmapsQueryHandler : IQueryHandler<GetPublicRoadmapsQuery, IReadOnlyList<PublicRoadmapsDto>>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetPublicRoadmapsQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<IReadOnlyList<PublicRoadmapsDto>>> Handle(GetPublicRoadmapsQuery request, CancellationToken cancellationToken)
    {
        var roadmaps = await _roadmapRepository.GetPublicRoadmapsAsync();

        var roadmapDtos = roadmaps.Select(roadmap => new PublicRoadmapsDto(
                roadmap.Id,
                roadmap.Title
            )).ToList();

        return Result.Success<IReadOnlyList<PublicRoadmapsDto>>(roadmapDtos);
    }
} 