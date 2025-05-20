using Application.Abstractions.Messaging;

namespace Application.Features.CompanyDashboard.Queries.GetCompanyDashboard;

public sealed record GetCompanyDashboardQuery(Guid UserId) : IQuery<CompanyDashboardDto>;
