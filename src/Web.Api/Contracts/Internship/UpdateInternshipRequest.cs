namespace Web.Api.Contracts.Internship;

public sealed record UpdateInternshipRequest(
    string Title,
    string Description,
    DateTime ApplicationDeadline
);