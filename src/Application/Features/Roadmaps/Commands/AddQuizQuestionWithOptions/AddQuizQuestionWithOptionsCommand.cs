using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Commands.AddQuizQuestionWithOptions
{
    public sealed record AddQuizQuestionWithOptionsCommand(
        Guid RoadmapId,
        Guid SectionId,
        Guid QuizId,
        string QuestionText,
        int QuestionPoints,
        List<QuizOptionRequest> Options
    ) : ICommand<Guid>;

    // Removed QuizOptionInput record as it's now redundant; use QuizOptionRequest from AddQuizQuestionWithOptionsRequest.cs
}
