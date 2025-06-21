using System.Collections.Generic;

namespace Application.Features.Roadmaps.Commands.AddQuizQuestionWithOptions
{
    public sealed record AddQuizQuestionWithOptionsRequest(
        string QuestionText,
        int QuestionPoints,
        List<QuizOptionRequest> Options
    );

    public sealed record QuizOptionRequest(string Text, bool IsCorrect);
}
