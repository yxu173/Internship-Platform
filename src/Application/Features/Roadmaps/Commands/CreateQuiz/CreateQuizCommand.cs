using Application.Abstractions.Messaging;

namespace Application.Features.Roadmaps.Commands.CreateQuiz;

public sealed record CreateQuizCommand(
    Guid RoadmapId,
    Guid SectionId,
    int PassingScore,
    bool IsRequired) : ICommand<Guid>; 