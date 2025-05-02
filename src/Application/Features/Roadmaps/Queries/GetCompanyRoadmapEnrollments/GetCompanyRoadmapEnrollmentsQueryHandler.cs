using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Aggregates.Roadmaps;

namespace Application.Features.Roadmaps.Queries.GetCompanyRoadmapEnrollments;

internal sealed class GetCompanyRoadmapEnrollmentsQueryHandler : IQueryHandler<GetCompanyRoadmapEnrollmentsQuery, IReadOnlyList<CompanyRoadmapEnrollmentDto>>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetCompanyRoadmapEnrollmentsQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<IReadOnlyList<CompanyRoadmapEnrollmentDto>>> Handle(GetCompanyRoadmapEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Roadmap> companyRoadmaps;
        if (request.RoadmapId.HasValue)
        {
            var specificRoadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId.Value, includeSections: true); // Need sections/items for count
            if (specificRoadmap == null || specificRoadmap.CompanyId != request.CompanyId)
            {
                return Result.Success((IReadOnlyList<CompanyRoadmapEnrollmentDto>)new List<CompanyRoadmapEnrollmentDto>());
            }
            companyRoadmaps = new List<Roadmap> { specificRoadmap };
        }
        else
        {
            // TODO: Optimize this - fetching ALL roadmaps with full structure might be heavy.
            companyRoadmaps = await _roadmapRepository.GetByCompanyIdAsync(request.CompanyId); 
        }

        var roadmapIds = companyRoadmaps.Select(r => r.Id).ToList();
        if (!roadmapIds.Any()) return Result.Success((IReadOnlyList<CompanyRoadmapEnrollmentDto>)new List<CompanyRoadmapEnrollmentDto>());

        var totalItemsPerRoadmap = companyRoadmaps.ToDictionary(r => r.Id, r => r.Sections?.Sum(s => s.Items?.Count ?? 0) ?? 0);

        var enrollments = await _roadmapRepository.GetEnrollmentsByRoadmapIdsAsync(roadmapIds);
        var enrollmentIds = enrollments.Select(e => e.Id).ToList();
        if (!enrollmentIds.Any()) return Result.Success((IReadOnlyList<CompanyRoadmapEnrollmentDto>)new List<CompanyRoadmapEnrollmentDto>());

        var allProgress = await _roadmapRepository.GetProgressByEnrollmentIdsAsync(enrollmentIds);
        var progressByEnrollmentId = allProgress.GroupBy(p => p.EnrollmentId).ToDictionary(g => g.Key, g => g.Count()); // Just need counts

        var dtos = new List<CompanyRoadmapEnrollmentDto>();
        foreach (var enrollment in enrollments)
        {
            var completedCount = progressByEnrollmentId.TryGetValue(enrollment.Id, out var count) ? count : 0;
            var totalItems = totalItemsPerRoadmap.TryGetValue(enrollment.RoadmapId, out var total) ? total : 0;
            double completionPercentage = totalItems > 0 ? (double)completedCount / totalItems * 100 : 0;

            var studentInfo = new StudentInfoForCompanyDto(enrollment.StudentId);

            dtos.Add(new CompanyRoadmapEnrollmentDto(
                enrollment.Id,
                enrollment.RoadmapId,
                studentInfo,
                enrollment.EnrolledAt,
                enrollment.PaymentStatus.ToString(),
                completionPercentage
            ));
        }

        return Result.Success<IReadOnlyList<CompanyRoadmapEnrollmentDto>>(dtos);
    }
}
