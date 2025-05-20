using Domain.Aggregates.Profiles;
using Domain.Enums;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Repositories;

public class CompanyDashboardRepository : ICompanyDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyDashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTotalInternshipsCountAsync(Guid companyId)
    {
        return await _context.Internships
            .Where(i => i.CompanyProfileId == companyId)
            .CountAsync();
    }

    public async Task<int> GetActiveInternshipsCountAsync(Guid companyId)
    {
        return await _context.Internships
            .Where(i => i.CompanyProfileId == companyId && i.IsActive && i.ApplicationDeadline > DateTime.UtcNow)
            .CountAsync();
    }

    public async Task<int> GetTotalApplicationsCountAsync(Guid companyId)
    {
        return await _context.Applications
            .Where(a => a.Internship.CompanyProfileId == companyId)
            .CountAsync();
    }

    public async Task<IDictionary<ApplicationStatus, int>> GetApplicationsByStatusAsync(Guid companyId)
    {
        return await _context.Applications
            .Where(a => a.Internship.CompanyProfileId == companyId)
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    public async Task<string> GetTopTechnologyAsync(Guid companyId)
    {
        var techFromRoadmaps = await _context.Roadmaps
            .Where(r => r.CompanyId == companyId)
            .GroupBy(r => r.Technology)
            .Select(g => new { Technology = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync();

        return techFromRoadmaps?.Technology ?? "No technology data available";
    }

    public async Task<decimal> GetTotalRevenueAsync(Guid companyId)
    {
        var roadmapIds = await _context.Roadmaps
            .Where(r => r.CompanyId == companyId)
            .Select(r => r.Id)
            .ToListAsync();

        if (!roadmapIds.Any())
            return 0;

        return await _context.Enrollments
            .Where(e => roadmapIds.Contains(e.RoadmapId) && 
                        e.PaymentStatus == PaymentStatus.Completed &&
                        e.AmountPaid.HasValue)
            .SumAsync(e => e.AmountPaid ?? 0);
    }

    public async Task<IReadOnlyList<TechUsageData>> GetTechnologyUsageAsync(Guid companyId)
    {
        var techList = await (from e in _context.Enrollments
                             join r in _context.Roadmaps on e.RoadmapId equals r.Id
                             where r.CompanyId == companyId
                             select r.Technology)
                            .ToListAsync();
        return techList
            .GroupBy(tech => tech)
            .Select(g => new TechUsageData(g.Key, g.Count()))
            .OrderByDescending(d => d.ApplicationsCount)
            .Take(10)
            .ToList();
    }

    public async Task<IReadOnlyList<MonthlyRevenueData>> GetMonthlyRevenueAsync(Guid companyId)
    {
        // Get roadmap IDs for this company
        var roadmapIds = await _context.Roadmaps
            .Where(r => r.CompanyId == companyId)
            .Select(r => r.Id)
            .ToListAsync();

        if (!roadmapIds.Any())
            return new List<MonthlyRevenueData>();

        var startDate = DateTime.UtcNow.AddMonths(-11);
        
        var enrollmentData = await _context.Enrollments
            .Where(e => roadmapIds.Contains(e.RoadmapId) && 
                        e.PaymentStatus == PaymentStatus.Completed &&
                        e.AmountPaid.HasValue &&
                        e.EnrolledAt >= startDate)
            .Select(e => new {
                Year = e.EnrolledAt.Year,
                Month = e.EnrolledAt.Month,
                Amount = e.AmountPaid ?? 0
            })
            .ToListAsync();
            
        var monthlyRevenue = enrollmentData
            .GroupBy(e => new { e.Year, e.Month })
            .Select(g => new MonthlyRevenueData(
                g.Key.Year,
                g.Key.Month,
                g.Sum(e => e.Amount)
            ))
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();

        return monthlyRevenue;
    }

    public async Task<IReadOnlyList<UniversityDistributionData>> GetUniversityDistributionAsync(Guid companyId)
    {
        var applicationData = await _context.Applications
            .Where(a => a.Internship.CompanyProfileId == companyId)
            .Join(
                _context.StudentProfiles,
                app => app.StudentProfileId,
                profile => profile.Id,
                (app, profile) => new { University = profile.University.ToString() }
            )
            .ToListAsync();
            
        return applicationData
            .GroupBy(x => x.University)
            .Select(g => new UniversityDistributionData(
                g.Key,
                g.Count()
            ))
            .OrderByDescending(u => u.StudentsCount)
            .Take(10)
            .ToList();
    }

    public async Task<double> GetAverageRoadmapCompletionRateAsync(Guid companyId)
    {
        var roadmapIds = await _context.Roadmaps
            .Where(r => r.CompanyId == companyId)
            .Select(r => r.Id)
            .ToListAsync();

        if (!roadmapIds.Any())
            return 0;

        var enrollments = await _context.Enrollments
            .Where(e => roadmapIds.Contains(e.RoadmapId))
            .ToListAsync();

        if (!enrollments.Any())
            return 0;

        double totalCompletionPercentage = 0;
        int enrollmentCount = 0;

        foreach (var enrollment in enrollments)
        {
            var roadmap = await _context.Roadmaps
                .Include(r => r.Sections)
                .ThenInclude(s => s.Items)
                .FirstOrDefaultAsync(r => r.Id == enrollment.RoadmapId);

            if (roadmap == null)
                continue;

            var progressItems = await _context.ResourceProgresses
                .Where(rp => rp.EnrollmentId == enrollment.Id)
                .CountAsync();

            var totalItems = roadmap.Sections.Sum(s => s.Items.Count);
            
            if (totalItems > 0)
            {
                totalCompletionPercentage += (double)progressItems / totalItems * 100;
                enrollmentCount++;
            }
        }

        return enrollmentCount > 0 ? totalCompletionPercentage / enrollmentCount : 0;
    }
}
