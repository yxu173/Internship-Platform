using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using SharedKernel;
using Domain.Aggregates.Roadmaps;
using System.Linq;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Queries.GetRoadmapsByCompanyId;

internal sealed class GetRoadmapsByCompanyIdQueryHandler : IQueryHandler<GetRoadmapsByCompanyIdQuery, IReadOnlyList<RoadmapDto>>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetRoadmapsByCompanyIdQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<IReadOnlyList<RoadmapDto>>> Handle(GetRoadmapsByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var roadmaps = await _roadmapRepository.GetByCompanyIdAsync(request.CompanyId);

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
                null,
                false,
                null
            )).ToList();

        return Result.Success<IReadOnlyList<RoadmapDto>>(roadmapDtos);
    }
} 