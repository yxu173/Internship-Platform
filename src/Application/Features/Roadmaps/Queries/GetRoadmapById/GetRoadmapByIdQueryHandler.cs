using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps;
using Domain.ValueObjects; 
using System.Linq; 
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Queries.GetRoadmapById;

internal sealed class GetRoadmapByIdQueryHandler : IQueryHandler<GetRoadmapByIdQuery, RoadmapDto>
{
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IStudentRepository _studentRepository;

    public GetRoadmapByIdQueryHandler(IRoadmapRepository roadmapRepository, IStudentRepository studentRepository)
    {
        _roadmapRepository = roadmapRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<RoadmapDto>> Handle(GetRoadmapByIdQuery request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, request.IncludeSections);

        if (roadmap is null)
        {
            return Result.Failure<RoadmapDto>(RoadmapErrors.RoadmapNotFound);
        }

        List<RoadmapSectionDto>? sectionDtos = null;
        if (request.IncludeSections && roadmap.Sections is not null)
        {
            var student = await _studentRepository.GetByUserIdAsync(request.UserId);
            var enrollment = student != null ? await _roadmapRepository.GetEnrollmentAsync(student.Id, request.RoadmapId) : null;
            var orderedSections = roadmap.Sections.OrderBy(s => s.Order).ToList();
            sectionDtos = new List<RoadmapSectionDto>();
            foreach (var section in orderedSections)
            {
                bool isAccessible = enrollment != null && enrollment.CanAccessSection(section.Id, orderedSections);
                if (isAccessible)
                {
                    sectionDtos.Add(new RoadmapSectionDto(
                        section.Id,
                        section.Title,
                        section.Order,
                        section.Quiz?.Id,
                        section.Items.Select(item => new RoadmapItemDto(
                            item.Title,
                            item.Resources.Select(r => new ResourceLinkDto(
                                r.Title,
                                r.Url,
                                r.Type.ToString()
                            )).ToList(),
                            item.Order
                        )).ToList()
                    ));
                }
                else
                {
                    sectionDtos.Add(new RoadmapSectionDto(
                        section.Id,
                        section.Title,
                        section.Order,
                        null,
                        null
                    ));
                }
            }
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
            sectionDtos
        );

        return roadmapDto;
    }
} 