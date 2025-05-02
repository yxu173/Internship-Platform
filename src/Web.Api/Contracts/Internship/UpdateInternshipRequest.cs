namespace Web.Api.Contracts.Internship;

public sealed record UpdateInternshipRequest(
    string Title,
    string About,
    string KeyResponsibilities,
    string Requirements,
    string Type,
    string WorkingModel,
    decimal Salary,
    string Currency,
    DateTime ApplicationDeadline
);