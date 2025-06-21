using Application.Abstractions.Messaging;

namespace Application.Features.Roadmaps.Commands.AddQuizOption;

public sealed record AddQuizOptionCommand(
    Guid RoadmapId,
    Guid SectionId,
    Guid QuizId,
    Guid QuestionId,
    string Text,
    bool IsCorrect) : ICommand<Guid>; 