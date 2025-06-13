using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps; // Needed for Roadmap
using Domain.ValueObjects; // Needed for ResourceLink
using System.Linq; // Added for Select
using System.Collections.Generic; // Added for List

namespace Application.Features.Roadmaps.Queries.GetRoadmapById;

internal sealed class GetRoadmapByIdQueryHandler : IQueryHandler<GetRoadmapByIdQuery, RoadmapDto>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetRoadmapByIdQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<RoadmapDto>> Handle(GetRoadmapByIdQuery request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, request.IncludeSections);

        if (roadmap is null)
        {
            return Result.Failure<RoadmapDto>(RoadmapErrors.RoadmapNotFound);
        }

        var roadmapDto = new RoadmapDto(
            roadmap.Id,
            roadmap.Title,
            roadmap.Description,
            roadmap.Technology,
            roadmap.IsPremium,
            roadmap.Price,
            roadmap.CompanyId,
            roadmap.CreatedAt,
            null, 
            request.IncludeSections && roadmap.Sections is not null
                ? roadmap.Sections.Select(section => new RoadmapSectionDto(
                    section.Id,
                    section.Title,
                    section.Order,
                    section.Items.Select(item => new RoadmapItemDto(
                        item.Title,
                        item.Resources.Select(r => new ResourceLinkDto(
                            r.Title, 
                            r.Url, 
                            r.Type.ToString() 
                            )).ToList(),
                        item.Order
                    )).ToList()
                )).ToList()
                : null
        );

        return roadmapDto;
    }
} 