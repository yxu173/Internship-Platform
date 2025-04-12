using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Repositories;
using SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Aggregates.Roadmaps;

namespace Application.Features.Roadmaps.Queries.GetStudentEnrollments;

internal sealed class GetStudentEnrollmentsQueryHandler : IQueryHandler<GetStudentEnrollmentsQuery, IReadOnlyList<EnrollmentSummaryDto>>
{
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IStudentRepository _studentRepository;

    public GetStudentEnrollmentsQueryHandler(IRoadmapRepository roadmapRepository, IStudentRepository studentRepository)
    {
        _roadmapRepository = roadmapRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<IReadOnlyList<EnrollmentSummaryDto>>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        var enrollments = await _roadmapRepository.GetEnrollmentsByStudentIdAsync(student.Id, includeRoadmap: true);
        if (enrollments == null || !enrollments.Any())
        {
            return Result.Success((IReadOnlyList<EnrollmentSummaryDto>)new List<EnrollmentSummaryDto>());
        }

        var allProgress = await _roadmapRepository.GetProgressByStudentIdAsync(student.Id);
        
        var progressByEnrollmentId = allProgress.GroupBy(p => p.EnrollmentId).ToDictionary(g => g.Key, g => g.ToList());

        var dtos = new List<EnrollmentSummaryDto>();
        foreach (var enrollment in enrollments)
        {
            if (enrollment.Roadmap == null)
            {
                continue; 
            }

            var completedCount = progressByEnrollmentId.TryGetValue(enrollment.Id, out var progressList) ? progressList.Count : 0;

            // TODO: Further optimize item count retrieval if GetByIdAsync is too slow
            var totalItems = enrollment.Roadmap.Sections?.Sum(s => s.Items?.Count ?? 0) ?? 0;
            double completionPercentage = totalItems > 0 ? (double)completedCount / totalItems * 100 : 0;
            
            var roadmapInfo = new RoadmapInfoForEnrollmentDto(
                enrollment.Roadmap.Id,
                enrollment.Roadmap.Title,
                enrollment.Roadmap.Technology,
                enrollment.Roadmap.CompanyId
            );

            dtos.Add(new EnrollmentSummaryDto(
                enrollment.Id,
                roadmapInfo,
                enrollment.EnrolledAt,
                enrollment.PaymentStatus.ToString(),
                completionPercentage
            ));
        }

        return Result.Success<IReadOnlyList<EnrollmentSummaryDto>>(dtos);
    }
} 