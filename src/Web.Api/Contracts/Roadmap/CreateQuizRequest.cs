namespace Web.Api.Contracts.Roadmap;

public sealed record CreateQuizRequest(
    int PassingScore,
    bool IsRequired
); 