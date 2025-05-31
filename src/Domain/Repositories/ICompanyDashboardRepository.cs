using Domain.Enums;
using SharedKernel;

namespace Domain.Repositories;

public interface ICompanyDashboardRepository
{
    Task<int> GetTotalInternshipsCountAsync(Guid companyId);
    Task<int> GetActiveInternshipsCountAsync(Guid companyId);
    Task<int> GetTotalApplicationsCountAsync(Guid companyId);
    Task<IDictionary<ApplicationStatus, int>> GetApplicationsByStatusAsync(Guid companyId);
    Task<string> GetTopTechnologyAsync(Guid companyId);
    Task<decimal> GetTotalRevenueAsync(Guid companyId);
    Task<IReadOnlyList<TechUsageData>> GetTechnologyUsageAsync(Guid companyId);
    Task<IReadOnlyList<MonthlyRevenueData>> GetMonthlyRevenueAsync(Guid companyId);
    Task<IReadOnlyList<UniversityDistributionData>> GetUniversityDistributionAsync(Guid companyId);
    Task<double> GetAverageRoadmapCompletionRateAsync(Guid companyId);
}

public record TechUsageData(string Technology, int ApplicationsCount);
public record MonthlyRevenueData(int Year, int Month, decimal Amount);
public record UniversityDistributionData(string University, int StudentsCount);
