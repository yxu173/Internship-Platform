using Application.Abstractions.Messaging;
using Application.Features.Internships.GetApplicationsByInternshipId;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.GetAcceptedApplications;

public sealed class AcceptedApplicationsByInternshipIdQueryHandler : IQueryHandler<AcceptedApplicationsByInternshipIdQuery, IReadOnlyList<ApplicationDto>>
{
    private readonly IInternshipRepository _internshipRepository;

    public AcceptedApplicationsByInternshipIdQueryHandler(IInternshipRepository internshipRepository)
    {
        _internshipRepository = internshipRepository;
    }

    public async Task<Result<IReadOnlyList<ApplicationDto>>> Handle(AcceptedApplicationsByInternshipIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var applications = await _internshipRepository.GetAcceptedApplicationsByInternshipIdAsync(request.InternshipId);

            if (applications == null || applications.Count == 0)
            {
                return Result.Success<IReadOnlyList<ApplicationDto>>(new List<ApplicationDto>());
            }

            var applicationDtos = applications.Select(a => new ApplicationDto(
                Id: a.Id,
                StudentProfileId: a.StudentProfileId,
                StudentName: a.StudentProfile.FullName,
                StudentTitle: "Student",
                Education: a.StudentProfile.University.ToString(),
                AppliedAt: a.AppliedAt,
                ResumeUrl: a.ResumeUrl,
                Status: a.Status.ToString(),
                DecisionDate: a.DecisionDate,
                FeedbackNotes: a.FeedbackNotes
            )).ToList();

            return Result.Success<IReadOnlyList<ApplicationDto>>(applicationDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<ApplicationDto>>(Error.BadRequest("Failed to get accepted applications", ex.Message));
        }
    }
} 