using System;
using Application.Abstractions.Messaging;
using MediatR;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompanyPosts;

public record GetCompanyPostsQuery(Guid UserId, int Page = 1, int PageSize = 5) : IQuery<CompanyPostsResponse>;

public sealed record CompanyPostsResponse(
    IReadOnlyList<CompanyPostDto> Posts,
    int TotalCount,
    int TotalPages,
    int CurrentPage);