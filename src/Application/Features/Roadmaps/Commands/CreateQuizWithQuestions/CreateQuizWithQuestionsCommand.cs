using MediatR;
using System;
using System.Collections.Generic;
using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.Commands.AddQuizQuestionWithOptions;

namespace Application.Features.Roadmaps.Commands.CreateQuizWithQuestions;

public sealed record CreateQuizWithQuestionsCommand(
    Guid RoadmapId,
    Guid SectionId,
    int PassingScore,
    bool IsRequired,
    List<QuestionQuizDto> Questions
    ) : ICommand<Guid>;

public record QuestionQuizDto(
    string Text,
    int Points,
    List<QuizOptionRequest> Options
);