using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;
using System;

namespace Application.Features.Roadmaps.Queries.GetStudentRoadmapProgress;

public record GetStudentRoadmapProgressQuery(
    Guid StudentId,
    Guid RoadmapId
) : IQuery<RoadmapProgressDto>; 