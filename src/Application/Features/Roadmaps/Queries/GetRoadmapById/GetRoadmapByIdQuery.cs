using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetRoadmapById;

public record GetRoadmapByIdQuery(Guid RoadmapId, Guid UserId, bool IncludeSections = false) : IQuery<RoadmapDto>; 