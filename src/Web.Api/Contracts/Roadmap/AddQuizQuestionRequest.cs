namespace Web.Api.Contracts.Roadmap;

public sealed record AddQuizQuestionRequest(
    string Text,
    int Points
); 