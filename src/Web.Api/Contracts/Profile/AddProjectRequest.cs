namespace Web.Api.Contracts.Profile;

public sealed record AddProjectRequest(
    string ProjectName,
    string Description,
    string ProjectUrl);