namespace Web.Api.Contracts.Profile;

public sealed record UpdateProjectRequest(
    string ProjectName,
    string Description,
    string ProjectUrl
);