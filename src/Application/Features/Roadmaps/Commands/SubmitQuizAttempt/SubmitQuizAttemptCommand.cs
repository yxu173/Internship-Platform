using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;

namespace Application.Features.Roadmaps.Commands.SubmitQuizAttempt;

public sealed record SubmitQuizAttemptCommand(
    Guid UserId,
    Guid RoadmapId,
    Guid SectionId,
    Guid QuizId,
    List<QuizAnswerDto> Answers
) : ICommand<QuizResultDto>; 