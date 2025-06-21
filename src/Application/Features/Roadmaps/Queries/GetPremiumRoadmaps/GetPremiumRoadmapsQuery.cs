using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.Queries.GetPremiumRoadmaps;
public sealed record GetPremiumRoadmapsQuery(): IQuery<IReadOnlyList<PublicRoadmapsDto>>; 