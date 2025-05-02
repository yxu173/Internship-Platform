using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetPublicRoadmaps;
 
public record GetPublicRoadmapsQuery(int Page = 1, int PageSize = 20) : IQuery<IReadOnlyList<RoadmapDto>>; 