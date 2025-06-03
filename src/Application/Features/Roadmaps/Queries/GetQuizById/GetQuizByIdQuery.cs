using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.Queries.GetQuizById;

public sealed record GetQuizByIdQuery(
    Guid RoadmapId,
    Guid SectionId,
    Guid QuizId) : IQuery<QuizDto>; 