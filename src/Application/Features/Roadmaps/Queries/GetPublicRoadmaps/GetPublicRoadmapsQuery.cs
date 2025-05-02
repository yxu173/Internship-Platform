using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetPublicRoadmaps;
 
public record GetPublicRoadmapsQuery() : IQuery<IReadOnlyList<PublicRoadmapsDto>>; 