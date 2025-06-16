using Application.Abstractions.Messaging;
using Domain.Enums;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Home.Queries.GetHomePageData;

public sealed class GetHomePageDataQueryHandler : IQueryHandler<GetHomePageDataQuery, HomePageDto>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IInternshipBookmarkRepository _internshipBookmarkRepository;
    private readonly IRoadmapBookmarkRepository _roadmapBookmarkRepository;
    private readonly IUserRepository _userRepository;

    public GetHomePageDataQueryHandler(
        IInternshipRepository internshipRepository,
        IRoadmapRepository roadmapRepository,
        IInternshipBookmarkRepository internshipBookmarkRepository,
        IRoadmapBookmarkRepository roadmapBookmarkRepository,
        IUserRepository userRepository)
    {
        _internshipRepository = internshipRepository;
        _roadmapRepository = roadmapRepository;
        _internshipBookmarkRepository = internshipBookmarkRepository;
        _roadmapBookmarkRepository = roadmapBookmarkRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<HomePageDto>> Handle(GetHomePageDataQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await _userRepository.GetByIdAsync(request.UserId);
            if (userProfile == null)
            {
                return Result.Failure<HomePageDto>(Error.NotFound("User not found",""));
            }

            var bookmarkedInternships = new List<Guid>();
            var bookmarkedRoadmaps = new List<Guid>();

            if (userProfile.StudentProfile != null)
            {
                var studentId = userProfile.StudentProfile.Id;
                var bookmarks = await _internshipBookmarkRepository.GetByStudentIdAsync(studentId, cancellationToken);
                bookmarkedInternships = bookmarks.Select(b => b.InternshipId).ToList();

                var roadmapBookmarks = await _roadmapBookmarkRepository.GetByStudentIdAsync(studentId, cancellationToken);
                bookmarkedRoadmaps = roadmapBookmarks.Select(b => b.RoadmapId).ToList();
            }

            var allInternships = await _internshipRepository.GetActiveInternshipsAsync();
            var recentInternships = allInternships
                .OrderByDescending(i => i.CreatedAt)
                .Take(10)
                .Select(i => new InternshipCardDto(
                    i.Id,
                    i.Title,
                    i.CompanyProfile?.CompanyName ?? "Unknown Company",
                    i.CompanyProfile?.LogoUrl ?? "",
                    i.Type.ToString(),
                    i.WorkingModel.ToString(),
                    i.Salary.Amount.ToString(),
                    i.Salary.Currency,
                    i.ApplicationDeadline,
                    i.Applications.Count,
                    bookmarkedInternships.Contains(i.Id)
                ))
                .ToList();

            var recommendedInternships = new List<InternshipCardDto>();
            
            if (userProfile.StudentProfile != null)
            {
                recommendedInternships = allInternships
                    .OrderByDescending(i => i.Applications.Count)
                    .Take(10)
                    .Select(i => new InternshipCardDto(
                        i.Id,
                        i.Title,
                        i.CompanyProfile?.CompanyName ?? "Unknown Company",
                        i.CompanyProfile?.LogoUrl ?? "",
                        i.Type.ToString(),
                        i.WorkingModel.ToString(),
                        i.Salary.Amount.ToString(),
                        i.Salary.Currency,
                        i.ApplicationDeadline,
                        i.Applications.Count,
                        bookmarkedInternships.Contains(i.Id)
                    ))
                    .ToList();
            }
            else
            {
                recommendedInternships = allInternships
                    .OrderByDescending(i => i.Applications.Count)
                    .Take(10)
                    .Select(i => new InternshipCardDto(
                        i.Id,
                        i.Title,
                        i.CompanyProfile?.CompanyName ?? "Unknown Company",
                        i.CompanyProfile?.LogoUrl ?? "",
                        i.Type.ToString(),
                        i.WorkingModel.ToString(),
                        i.Salary.Amount.ToString(),
                        i.Salary.Currency,
                        i.ApplicationDeadline,
                        i.Applications.Count,
                        bookmarkedInternships.Contains(i.Id)
                    ))
                    .ToList();
            }

            var regularInternships = allInternships
                .Where(i => i.IsActive)
                .OrderBy(i => i.ApplicationDeadline)
                .Take(10)
                .Select(i => new InternshipCardDto(
                    i.Id,
                    i.Title,
                    i.CompanyProfile?.CompanyName ?? "Unknown Company",
                    i.CompanyProfile?.LogoUrl ?? "",
                    i.Type.ToString(),
                    i.WorkingModel.ToString(),
                    i.Salary.Amount.ToString(),
                    i.Salary.Currency,
                    i.ApplicationDeadline,
                    i.Applications.Count,
                    bookmarkedInternships.Contains(i.Id)
                ))
                .ToList();
            
            var realWorldProjects = allInternships
                .Where(i => i.Type == InternshipType.ProjectBased)
                .Take(10)
                .Select(i => new ProjectCardDto(
                    i.Id,
                    i.Title,
                    i.CompanyProfile?.CompanyName ?? "Unknown Company",
                    i.CompanyProfile?.LogoUrl ?? "",
                    i.Type.ToString(),
                    "Various Technologies",
                    i.Applications.Count,
                    bookmarkedInternships.Contains(i.Id)
                ))
                .ToList();

            // Get roadmaps
            var roadmaps = await _roadmapRepository.GetPremiumRoadmapsAsync();
            var roadmapDtos = roadmaps
                .Where(r => r.CompanyId != null)
                .Take(10)
                .Select(r => new RoadmapCardDto(
                    r.Id,
                    r.Title,
                    r.Company?.CompanyName ?? "Unknown Company",
                    r.Company?.LogoUrl ?? "",
                    r.Technology ?? "General",
                    "Technology",
                    r.Enrollments?.Count ?? 0,
                    bookmarkedRoadmaps.Contains(r.Id)
                ))
                .ToList();

            return Result.Success(new HomePageDto(
                recentInternships,
                recommendedInternships,
                regularInternships,
                realWorldProjects,
                roadmapDtos
            ));
        }
        catch (Exception ex)
        {
            return Result.Failure<HomePageDto>(Error.BadRequest("Failed to get homepage data", ex.Message));
        }
    }
}
