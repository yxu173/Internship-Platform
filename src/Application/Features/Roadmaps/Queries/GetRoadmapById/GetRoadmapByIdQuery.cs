using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetRoadmapById;

public record GetRoadmapByIdQuery(Guid RoadmapId, bool IncludeSections = false) : IQuery<RoadmapDto>; 