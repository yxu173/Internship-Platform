using Domain.Enums;

namespace Application.Features.CompanyDashboard.Queries.GetCompanyDashboard;

public sealed record CompanyDashboardDto(
    int TotalInternships,
    int ActiveInternships,
    int TotalApplications,
    IDictionary<ApplicationStatus, int> ApplicationsByStatus,
    string TopTechnology,
    decimal TotalRevenue,
    double AverageCompletionRate,
    IReadOnlyList<TechUsageDto> TechUsage,
    IReadOnlyList<MonthlyRevenueDto> RevenuePerMonth,
    IReadOnlyList<UniversityDistributionDto> UniversityDistribution
);

public sealed record TechUsageDto(string Technology, int ApplicationsCount);

public sealed record MonthlyRevenueDto(int Year, int Month, decimal Amount);

public sealed record UniversityDistributionDto(string University, int StudentsCount);
