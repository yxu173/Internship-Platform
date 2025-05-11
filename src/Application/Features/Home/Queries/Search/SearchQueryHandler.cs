using Application.Abstractions.Messaging;
using Application.Features.Home.Queries.GetHomePageData;
using Domain.Repositories;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Home.Queries.Search;

public sealed class SearchQueryHandler : IQueryHandler<SearchQuery, SearchResultsDto>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IInternshipBookmarkRepository _internshipBookmarkRepository;
    private readonly IRoadmapBookmarkRepository _roadmapBookmarkRepository;
    private readonly IUserRepository _userRepository;

    public SearchQueryHandler(
        IInternshipRepository internshipRepository,
        ICompanyRepository companyRepository,
        IRoadmapRepository roadmapRepository,
        IInternshipBookmarkRepository internshipBookmarkRepository,
        IRoadmapBookmarkRepository roadmapBookmarkRepository,
        IUserRepository userRepository)
    {
        _internshipRepository = internshipRepository;
        _companyRepository = companyRepository;
        _roadmapRepository = roadmapRepository;
        _internshipBookmarkRepository = internshipBookmarkRepository;
        _roadmapBookmarkRepository = roadmapBookmarkRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<SearchResultsDto>> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var bookmarkedInternships = new List<Guid>();
            var bookmarkedRoadmaps = new List<Guid>();

            if (request.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdWithStudentProfileAsync(request.UserId.Value, cancellationToken);
                if (user?.StudentProfile != null)
                {
                    var studentId = user.StudentProfile.Id;
                    
                    var studentBookmarks = await _internshipBookmarkRepository.GetByStudentIdAsync(studentId, cancellationToken);
                    bookmarkedInternships = studentBookmarks.Select(b => b.InternshipId).ToList();

                    var roadmapBookmarks = await _roadmapBookmarkRepository.GetByStudentIdAsync(studentId, cancellationToken);
                    bookmarkedRoadmaps = roadmapBookmarks.Select(b => b.RoadmapId).ToList();
                }
            }

            IReadOnlyList<InternshipCardDto> internships = Array.Empty<InternshipCardDto>();
            IReadOnlyList<CompanyCardDto> companies = Array.Empty<CompanyCardDto>();
            IReadOnlyList<RoadmapCardDto> roadmaps = Array.Empty<RoadmapCardDto>();
            
            int totalInternships = 0;
            int totalCompanies = 0;
            int totalRoadmaps = 0;

            if (request.Type == SearchType.All || request.Type == SearchType.Internships)
            {
                var searchResults = await _internshipRepository.SearchInternshipsAsync(
                    request.SearchTerm, 
                    request.Page, 
                    request.PageSize,
                    cancellationToken);
                
                totalInternships = searchResults.TotalCount;
                
                internships = searchResults.Items
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

            if (request.Type == SearchType.All || request.Type == SearchType.Companies)
            {
                var searchResults = await _companyRepository.SearchCompaniesAsync(
                    request.SearchTerm, 
                    request.Page, 
                    request.PageSize,
                    cancellationToken);
                
                totalCompanies = searchResults.TotalCount;
                
                companies = searchResults.Items
                    .Select(c => new CompanyCardDto(
                        c.Id,
                        c.CompanyName,
                        c.LogoUrl ?? "",
                        c.Industry ?? "General",
                        c.Internships?.Count(i => i.IsActive) ?? 0,
                        $"{(c.Address?.Governorate != null ? c.Address.Governorate.ToString() : "")}, {c.Address?.City ?? ""}".Trim(' ', ',')
                    ))
                    .ToList();
            }

            // Search roadmaps if requested
            if (request.Type == SearchType.All || request.Type == SearchType.Roadmaps)
            {
                var searchResults = await _roadmapRepository.SearchRoadmapsAsync(
                    request.SearchTerm, 
                    request.Page, 
                    request.PageSize,
                    cancellationToken);
                
                totalRoadmaps = searchResults.TotalCount;
                
                roadmaps = searchResults.Items
                    .Select(r => new RoadmapCardDto(
                        r.Id,
                        r.Title,
                        r.Company?.CompanyName ?? "Unknown Company",
                        r.Company?.LogoUrl ?? "",
                        r.Technology,
                        "General", 
                        0, 
                        bookmarkedRoadmaps.Contains(r.Id) 
                    ))
                    .ToList();
            }

            int totalItems = request.Type switch
            {
                SearchType.Internships => totalInternships,
                SearchType.Companies => totalCompanies,
                SearchType.Roadmaps => totalRoadmaps,
                _ => totalInternships + totalCompanies + totalRoadmaps
            };
            
            int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

            return Result.Success(new SearchResultsDto(
                Internships: internships,
                Companies: companies,
                Roadmaps: roadmaps,
                TotalInternships: totalInternships,
                TotalCompanies: totalCompanies,
                TotalRoadmaps: totalRoadmaps,
                CurrentPage: request.Page,
                PageSize: request.PageSize,
                TotalPages: totalPages,
                SearchTerm: request.SearchTerm,
                SearchType: request.Type
            ));
        }
        catch (Exception ex)
        {
            return Result.Failure<SearchResultsDto>(new Error("Search.Failed", ex.Message, ErrorType.Failure));
        }
    }
}
