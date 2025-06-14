using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetSectionById;

public sealed class GetSectionByIdQueryHandler : IQueryHandler<GetSectionByIdQuery, RoadmapSectionDto>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetSectionByIdQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<RoadmapSectionDto>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var section =  await _roadmapRepository.GetSectionByIdAsync(request.SectionId);
        
        if (section == null)
        {
            return Result.Failure<RoadmapSectionDto>(RoadmapErrors.SectionNotFound);
        }

        return Result.Success(new RoadmapSectionDto(
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
            )).ToList()));
    }
}