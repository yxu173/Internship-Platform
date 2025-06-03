namespace Web.Api.Contracts.Roadmap;

public sealed record AddQuizOptionRequest(
    string Text,
    bool IsCorrect
); 