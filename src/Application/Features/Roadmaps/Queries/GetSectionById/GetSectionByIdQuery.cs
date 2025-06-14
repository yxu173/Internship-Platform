using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.Queries.GetSectionById;

public sealed record GetSectionByIdQuery(Guid SectionId) : IQuery<RoadmapSectionDto>;