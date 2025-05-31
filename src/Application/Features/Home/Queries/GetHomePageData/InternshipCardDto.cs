using Domain.Enums;

namespace Application.Features.Home.Queries.GetHomePageData;

public sealed record InternshipCardDto(
    Guid Id,
    string Title,
    string CompanyName,
    string CompanyLogoUrl,
    string Type,
    string WorkingModel,
    string Salary,
    string Currency,
    DateTime ApplicationDeadline,
    int ApplicationCount,
    bool IsBookmarked
);
