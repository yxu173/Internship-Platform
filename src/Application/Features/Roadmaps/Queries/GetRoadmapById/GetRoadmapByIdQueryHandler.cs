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
        RoadmapSectionDto? firstSectionWithQuiz = null;
        bool isEnrolled = false;
        if (request.IncludeSections && roadmap.Sections is not null)
        {
            var student = await _studentRepository.GetByUserIdAsync(request.UserId);
            if (student != null)
            {
                isEnrolled = await _roadmapRepository.HasEnrollmentAsync(student.Id, request.RoadmapId);
            }
            var orderedSections = roadmap.Sections.OrderBy(s => s.Order).ToList();
            sectionDtos = new List<RoadmapSectionDto>();
            foreach (var section in orderedSections)
            {
                bool isAccessible = false;
                var enrollment = student != null ? await _roadmapRepository.GetEnrollmentAsync(student.Id, request.RoadmapId) : null;
                if (enrollment != null)
                {
                    isAccessible = enrollment.CanAccessSection(section.Id, orderedSections);
                }
                var sectionDto = isAccessible
                    ? new RoadmapSectionDto(
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
                    )
                    : new RoadmapSectionDto(
                        section.Id,
                        section.Title,
                        section.Order,
                        null,
                        null
                    );
                sectionDtos.Add(sectionDto);
            }
            // For free roadmaps, adjust logic for FirstSectionWithQuiz
            if (!roadmap.IsPremium)
            {
                if (!isEnrolled)
                {
                    // User not enrolled: return the first section (by order), even if it has no quiz
                    var firstSection = orderedSections.OrderBy(s => s.Order).FirstOrDefault();
                    if (firstSection != null)
                    {
                        firstSectionWithQuiz = new RoadmapSectionDto(
                            firstSection.Id,
                            firstSection.Title,
                            firstSection.Order,
                            firstSection.Quiz?.Id,
                            firstSection.Items.Select(item => new RoadmapItemDto(
                                item.Title,
                                item.Resources.Select(r => new ResourceLinkDto(
                                    r.Title,
                                    r.Url,
                                    r.Type.ToString()
                                )).ToList(),
                                item.Order
                            )).ToList()
                        );
                    }
                }
                else
                {
                    // User enrolled: return the first section with a quiz (old logic)
                    var firstWithQuiz = orderedSections
                        .Where(s => s.Quiz != null)
                        .OrderBy(s => s.Order)
                        .FirstOrDefault();
                    if (firstWithQuiz != null)
                    {
                        firstSectionWithQuiz = new RoadmapSectionDto(
                            firstWithQuiz.Id,
                            firstWithQuiz.Title,
                            firstWithQuiz.Order,
                            firstWithQuiz.Quiz?.Id,
                            firstWithQuiz.Items.Select(item => new RoadmapItemDto(
                                item.Title,
                                item.Resources.Select(r => new ResourceLinkDto(
                                    r.Title,
                                    r.Url,
                                    r.Type.ToString()
                                )).ToList(),
                                item.Order
                            )).ToList()
                        );
                    }
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
            sectionDtos,
            isEnrolled,
            firstSectionWithQuiz
        );

        return roadmapDto;
    }
} 