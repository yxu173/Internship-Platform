using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Aggregates.Roadmaps;

namespace Application.Features.Roadmaps.Queries.GetStudentRoadmapProgress;

internal sealed class GetStudentRoadmapProgressQueryHandler : IQueryHandler<GetStudentRoadmapProgressQuery, RoadmapProgressDto>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetStudentRoadmapProgressQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<RoadmapProgressDto>> Handle(GetStudentRoadmapProgressQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _roadmapRepository.GetEnrollmentAsync(request.StudentId, request.RoadmapId);
        if (enrollment is null)
        {
            return Result.Failure<RoadmapProgressDto>(RoadmapErrors.EnrollmentNotFound);
        }

        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true); // Need items too
        if (roadmap is null)
        {
            return Result.Failure<RoadmapProgressDto>(RoadmapErrors.RoadmapNotFound);
        }

        var progressRecords = await _roadmapRepository.GetProgressByEnrollmentIdAsync(enrollment.Id); 
        var completedItemIds = progressRecords.Select(p => p.ItemId).ToHashSet();

        var sectionDtos = new List<RoadmapSectionProgressDto>();
        foreach (var section in roadmap.Sections.OrderBy(s => s.Order))
        {
            var itemDtos = new List<RoadmapItemProgressDto>();
            foreach (var item in section.Items.OrderBy(i => i.Order))
            {
                var resourceDtos = item.Resources
                    .Select(r => new ResourceLinkDto(r.Title, r.Url, r.Type.ToString()))
                    .ToList();

                itemDtos.Add(new RoadmapItemProgressDto(
                    item.Id,
                    item.Title,
                    item.Description,
                    item.Type.ToString(),
                    resourceDtos,
                    item.Order,
                    completedItemIds.Contains(item.Id)
                ));
            }
            sectionDtos.Add(new RoadmapSectionProgressDto(
                section.Id,
                section.Title,
                section.Order,
                itemDtos
            ));
        }

        var totalItems = roadmap.Sections.Sum(s => s.Items.Count);
        double overallCompletionPercentage = totalItems > 0 ? (double)completedItemIds.Count / totalItems * 100 : 0;

        var resultDto = new RoadmapProgressDto(
            roadmap.Id,
            roadmap.Title,
            roadmap.Description,
            roadmap.Technology,
            roadmap.CompanyId,
            enrollment.EnrolledAt,
            enrollment.PaymentStatus.ToString(),
            overallCompletionPercentage,
            sectionDtos
        );

        return Result.Success(resultDto);
    }
} 