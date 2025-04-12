using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using SharedKernel;
using Domain.Aggregates.Roadmaps;
using System.Linq;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Queries.GetPublicRoadmaps;

internal sealed class GetPublicRoadmapsQueryHandler : IQueryHandler<GetPublicRoadmapsQuery, IReadOnlyList<RoadmapDto>>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetPublicRoadmapsQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<IReadOnlyList<RoadmapDto>>> Handle(GetPublicRoadmapsQuery request, CancellationToken cancellationToken)
    {
        var roadmaps = await _roadmapRepository.GetPublicRoadmapsAsync(request.Page, request.PageSize);

        var roadmapDtos = roadmaps.Select(roadmap => new RoadmapDto(
                roadmap.Id,
                roadmap.Title,
                roadmap.Description,
                roadmap.Technology,
                roadmap.IsPremium,
                roadmap.Price,
                roadmap.CompanyId,
                roadmap.CreatedAt,
                null, 
                null 
            )).ToList();

        return Result.Success<IReadOnlyList<RoadmapDto>>(roadmapDtos);
    }
} 