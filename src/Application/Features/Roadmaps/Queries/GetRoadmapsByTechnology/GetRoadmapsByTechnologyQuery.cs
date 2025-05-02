using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetRoadmapsByTechnology;
 
public record GetRoadmapsByTechnologyQuery(string Technology) : IQuery<IReadOnlyList<RoadmapDto>>; 