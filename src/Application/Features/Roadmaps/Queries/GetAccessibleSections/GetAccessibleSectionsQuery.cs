using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.Queries.GetAccessibleSections;

public sealed record GetAccessibleSectionsQuery(
    Guid UserId,
    Guid RoadmapId) : IQuery<List<AccessibleSectionDto>>; 