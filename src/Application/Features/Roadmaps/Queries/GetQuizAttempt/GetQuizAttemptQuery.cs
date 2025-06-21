using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.Queries.GetQuizAttempt;

public record GetQuizAttemptQuery(Guid UserId, Guid QuizId) : IQuery<GetQuizAttemptResponse>;