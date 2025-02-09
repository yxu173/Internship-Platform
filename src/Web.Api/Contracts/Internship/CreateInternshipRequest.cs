namespace Web.Api.Contracts.Internship;

public sealed record CreateInternshipRequest(
    string Title,
    string Description,
    string Type,
    DateTime StartDate,
    DateTime EndDate,
    DateTime ApplicationDeadline
);
