using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompanyPosts;

public class GetCompanyPostsQueryHandler : IQueryHandler<GetCompanyPostsQuery, CompanyPostsResponse>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IRoadmapBookmarkRepository _roadmapBookmarkRepository;
    private readonly IInternshipBookmarkRepository _internshipBookmarkRepository;

    public GetCompanyPostsQueryHandler(
        ICompanyRepository companyRepository,
        IInternshipRepository internshipRepository,
        IRoadmapRepository roadmapRepository,
        IRoadmapBookmarkRepository roadmapBookmarkRepository,
        IInternshipBookmarkRepository internshipBookmarkRepository)
    {
        _companyRepository = companyRepository;
        _internshipRepository = internshipRepository;
        _roadmapRepository = roadmapRepository;
        _roadmapBookmarkRepository = roadmapBookmarkRepository;
        _internshipBookmarkRepository = internshipBookmarkRepository;
    }

    public async Task<Result<CompanyPostsResponse>> Handle(GetCompanyPostsQuery request,
        CancellationToken cancellationToken)
    {
        var companyProfileResult =
            await _companyRepository.GetByUserIdAsync<Domain.Aggregates.Profiles.CompanyProfile>(request.UserId,
                cp => cp);
        if (!companyProfileResult.IsSuccess || companyProfileResult.Value is null)
        {
            return Result.Failure<CompanyPostsResponse>(Error.NullValue);
        }

        var companyProfile = companyProfileResult.Value;

        var internships = await _internshipRepository.GetByCompanyIdAsync(companyProfile.Id);

        var roadmaps = await _roadmapRepository.GetByCompanyIdAsync(companyProfile.Id);

        var internshipPosts = internships.Select(i => new CompanyPostDto
        {
            Id = i.Id,
            Title = i.Title,
            Type = i.Type == InternshipType.Traditional ? "Internship" : "Project",
            IsActive = i.IsActive,
            Status = i.IsActive && i.ApplicationDeadline > DateTime.UtcNow ? "Active" : "Expire",
            ApplicationCount = i.Applications.Count,
            BookmarkCount = 0,
            DaysRemaining = i.IsActive && i.ApplicationDeadline > DateTime.UtcNow
                ? $"{(i.ApplicationDeadline - DateTime.UtcNow).Days} days remaining"
                : $"Dec {i.ApplicationDeadline.Day}, {i.ApplicationDeadline.Year}"
        }).ToList();

        var roadmapPosts = roadmaps.Select(r => new CompanyPostDto
        {
            Id = r.Id,
            Title = r.Title,
            Type = "Roadmap",
            IsActive = true,
            Status = "Active",
            ApplicationCount = r.Enrollments.Count,
            BookmarkCount = 0,
            DaysRemaining = $"{r.Sections.Count} sections"
        }).ToList();

        var allPosts = internshipPosts.Concat(roadmapPosts).ToList();

        var internshipIds = internshipPosts.Select(p => p.Id).ToList();
        if (internshipIds.Any())
        {
            var internshipBookmarkCounts =
                await _internshipBookmarkRepository.GetBookmarkCountsForInternshipsAsync(internshipIds);
            foreach (var post in internshipPosts)
            {
                if (internshipBookmarkCounts.TryGetValue(post.Id, out var count))
                {
                    post.BookmarkCount = count;
                }
            }
        }

        var roadmapIds = roadmapPosts.Select(p => p.Id).ToList();
        if (roadmapIds.Any())
        {
            var roadmapBookmarkCounts = await _roadmapBookmarkRepository.GetBookmarkCountsForRoadmapsAsync(roadmapIds);
            foreach (var post in roadmapPosts)
            {
                if (roadmapBookmarkCounts.TryGetValue(post.Id, out var count))
                {
                    post.BookmarkCount = count;
                }
            }
        }

        allPosts = allPosts.OrderByDescending(p => p.IsActive)
            .ThenByDescending(p => p.DaysRemaining.Contains("days") ? int.Parse(p.DaysRemaining.Split(' ')[0]) : 0)
            .ToList();

        var totalCount = allPosts.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var paginatedPosts = allPosts
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var response = new CompanyPostsResponse
        (
            paginatedPosts,
            totalCount,
            totalPages,
            request.Page
        );

        return Result.Success(response);
    }
}