using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using SharedKernel;
using Domain.Aggregates.Roadmaps; // Needed for Roadmap
using System.Linq; // Needed for Select
using System.Collections.Generic; // Needed for List

namespace Application.Features.Roadmaps.Queries.GetRoadmapsByTechnology;

internal sealed class GetRoadmapsByTechnologyQueryHandler : IQueryHandler<GetRoadmapsByTechnologyQuery, IReadOnlyList<RoadmapDto>>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetRoadmapsByTechnologyQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<IReadOnlyList<RoadmapDto>>> Handle(GetRoadmapsByTechnologyQuery request, CancellationToken cancellationToken)
    {
        var roadmaps = await _roadmapRepository.GetByTechnologyAsync(request.Technology);

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