using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetRoadmapsByCompanyId;
 
public record GetRoadmapsByCompanyIdQuery(Guid CompanyId) : IQuery<IReadOnlyList<RoadmapDto>>; 