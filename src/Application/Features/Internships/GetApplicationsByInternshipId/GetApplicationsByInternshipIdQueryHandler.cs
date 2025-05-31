using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.GetApplicationsByInternshipId;

public sealed class
    GetApplicationsByInternshipIdQueryHandler : IQueryHandler<GetApplicationsByInternshipIdQuery,
    IReadOnlyList<ApplicationDto>>
{
    private readonly IInternshipRepository _internshipRepository;

    public GetApplicationsByInternshipIdQueryHandler(IInternshipRepository internshipRepository)
    {
        _internshipRepository = internshipRepository;
    }

    public async Task<Result<IReadOnlyList<ApplicationDto>>> Handle(GetApplicationsByInternshipIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var applications = await _internshipRepository.GetApplicationsByInternshipIdAsync(request.InternshipId);

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
            return Result.Failure<IReadOnlyList<ApplicationDto>>(Error.BadRequest("Failed to get applications",
                ex.Message));
        }
    }
}