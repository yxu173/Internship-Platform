using Application.Abstractions.Messaging;
using Application.Abstractions.Pagination;
using Domain.Aggregates.Internships;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Bookmarks.Queries.GetBookmarkedItems;

internal sealed class GetBookmarkedItemsQueryHandler
    : IQueryHandler<GetBookmarkedItemsQuery, PagedList<BookmarkedItemResponse>>
{
    private readonly IInternshipBookmarkRepository _internshipBookmarkRepository;
    private readonly IRoadmapBookmarkRepository _roadmapBookmarkRepository;
    private readonly IStudentRepository _studentRepository;

    public GetBookmarkedItemsQueryHandler(
        IInternshipBookmarkRepository internshipBookmarkRepository, 
        IRoadmapBookmarkRepository roadmapBookmarkRepository, IStudentRepository studentRepository)
    {
        _internshipBookmarkRepository = internshipBookmarkRepository;
        _roadmapBookmarkRepository = roadmapBookmarkRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<PagedList<BookmarkedItemResponse>>> Handle(
        GetBookmarkedItemsQuery request, 
        CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        if (student == null)
        {
            return Result.Failure<PagedList<BookmarkedItemResponse>>(BookmarkErrors.InvalidStudentId);
        }
        
        var internshipBookmarks = await _internshipBookmarkRepository.GetByStudentIdAsync(student.Id, cancellationToken);
        var roadmapBookmarks     = await _roadmapBookmarkRepository.GetByStudentIdAsync(student.Id, cancellationToken);

        var mappedInternships = internshipBookmarks
            .Where(b => b.Internship is not null && b.Internship.CompanyProfile is not null)
            .Select(b => new BookmarkedItemResponse(
                b.InternshipId,
                b.Id,
                BookmarkType.Internship,
                b.Internship!.Title,
                b.Internship.CompanyProfile!.CompanyName,
                b.Internship.CompanyProfile.LogoUrl,
                CreateInternshipTags(b.Internship),
                b.Internship.ApplicationDeadline > DateTime.UtcNow,
                b.Internship.ApplicationDeadline > DateTime.UtcNow ? "Apply Now" : "Deadline Expired",
                b.CreatedAt
            ));

        var mappedRoadmaps = roadmapBookmarks
            .Where(b => b.Roadmap is not null && b.Roadmap.Company is not null)
            .Select(b => new BookmarkedItemResponse(
                b.RoadmapId,
                b.Id,
                BookmarkType.Roadmap,
                b.Roadmap!.Title,
                b.Roadmap.Company!.CompanyName,
                b.Roadmap.Company.LogoUrl,
                new List<string> { b.Roadmap.Technology },
                true,
                "View Roadmap",
                b.CreatedAt
            ));

        var combined = mappedInternships
            .Concat(mappedRoadmaps)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();

        var pagedBookmarks = PagedList<BookmarkedItemResponse>.Create(
            combined,
            request.PageNumber,
            request.PageSize);

        return Result.Success(pagedBookmarks);
    }
    
    private static List<string> CreateInternshipTags(Internship internship)
    {
        //TODO: Add Tags Here
        var tags = new List<string>();
        if (internship.IsRemote()) tags.Add("Remote");
        tags.Add(internship.Type.ToString());
        return tags;
    }
} 