using Application.Abstractions.Messaging;

namespace Application.Features.Roadmaps.Commands.AddQuizQuestion;

public sealed record AddQuizQuestionCommand(
    Guid RoadmapId,
    Guid SectionId,
    Guid QuizId,
    string Text,
    int Points) : ICommand<Guid>; 