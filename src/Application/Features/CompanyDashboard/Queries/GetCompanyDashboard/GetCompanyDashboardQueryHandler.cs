using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyDashboard.Queries.GetCompanyDashboard;

public sealed class GetCompanyDashboardQueryHandler : IQueryHandler<GetCompanyDashboardQuery, CompanyDashboardDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyDashboardRepository _dashboardRepository;

    public GetCompanyDashboardQueryHandler(
        ICompanyRepository companyRepository,
        ICompanyDashboardRepository dashboardRepository)
    {
        _companyRepository = companyRepository;
        _dashboardRepository = dashboardRepository;
    }

    public async Task<Result<CompanyDashboardDto>> Handle(GetCompanyDashboardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyRepository.GetCompanyByIdAsync(request.UserId);
            if (company == null)
            {
                return Result.Failure<CompanyDashboardDto>(Error.NotFound("Company not found", "The company profile was not found for the provided user ID."));
            }

            var companyId = company.Id;

            var totalInternships = await _dashboardRepository.GetTotalInternshipsCountAsync(companyId);
            var activeInternships = await _dashboardRepository.GetActiveInternshipsCountAsync(companyId);
            var totalApplications = await _dashboardRepository.GetTotalApplicationsCountAsync(companyId);
            var applicationsByStatus = await _dashboardRepository.GetApplicationsByStatusAsync(companyId);
            var topTechnology = await _dashboardRepository.GetTopTechnologyAsync(companyId);
            var totalRevenue = await _dashboardRepository.GetTotalRevenueAsync(companyId);
            var averageCompletionRate = await _dashboardRepository.GetAverageRoadmapCompletionRateAsync(companyId);

            var techUsage = await _dashboardRepository.GetTechnologyUsageAsync(companyId);
            var monthlyRevenue = await _dashboardRepository.GetMonthlyRevenueAsync(companyId);
            var universityDistribution = await _dashboardRepository.GetUniversityDistributionAsync(companyId);

            var techUsageDtos = techUsage.Select(t => new TechUsageDto(t.Technology, t.ApplicationsCount)).ToList();
            var monthlyRevenueDtos = monthlyRevenue.Select(m => new MonthlyRevenueDto(m.Year, m.Month, m.Amount)).ToList();
            var universityDistributionDtos = universityDistribution.Select(u => new UniversityDistributionDto(u.University, u.StudentsCount)).ToList();

            var dashboardDto = new CompanyDashboardDto(
                TotalInternships: totalInternships,
                ActiveInternships: activeInternships,
                TotalApplications: totalApplications,
                ApplicationsByStatus: applicationsByStatus,
                TopTechnology: topTechnology,
                TotalRevenue: totalRevenue,
                AverageCompletionRate: averageCompletionRate,
                TechUsage: techUsageDtos,
                RevenuePerMonth: monthlyRevenueDtos,
                UniversityDistribution: universityDistributionDtos
            );

            return Result.Success(dashboardDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<CompanyDashboardDto>(Error.BadRequest("Dashboard error", ex.Message));
        }
    }
}
