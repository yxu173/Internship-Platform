using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetAccessibleSections;

public sealed class GetAccessibleSectionsQueryHandler : IQueryHandler<GetAccessibleSectionsQuery, List<AccessibleSectionDto>>
{
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IStudentRepository _studentRepository;

    public GetAccessibleSectionsQueryHandler(IRoadmapRepository roadmapRepository, IStudentRepository studentRepository)
    {
        _roadmapRepository = roadmapRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<List<AccessibleSectionDto>>> Handle(GetAccessibleSectionsQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);
        if (roadmap == null)
        {
            return Result.Failure<List<AccessibleSectionDto>>(RoadmapErrors.RoadmapNotFound);
        }

        var enrollment = await _roadmapRepository.GetEnrollmentAsync(student.Id, request.RoadmapId);
        if (enrollment == null)
        {
            return Result.Failure<List<AccessibleSectionDto>>(RoadmapErrors.EnrollmentNotFound);
        }

        var orderedSections = roadmap.Sections.OrderBy(s => s.Order).ToList();
        
        var sectionDtos = new List<AccessibleSectionDto>();
        foreach (var section in orderedSections)
        {
            bool isAccessible = enrollment.CanAccessSection(section.Id, orderedSections);
            bool quizPassed = enrollment.SectionProgress.Any(sp => sp.SectionId == section.Id && sp.QuizPassed);
            int completionPercentage = enrollment.GetSectionCompletionPercentage(section.Id, section);
            
            bool hasQuiz = section.Quiz != null;
            
            sectionDtos.Add(new AccessibleSectionDto(
                section.Id,
                section.Title,
                section.Order,
                isAccessible,
                hasQuiz,
                quizPassed,
                completionPercentage
            ));
        }

        return Result.Success(sectionDtos);
    }
} 