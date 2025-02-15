namespace Web.Api.Contracts.Internship;

public sealed record CreateInternshipRequest(
    string Title,
    string About,
    string KeyResponsibilities,
    string Requirements,
    string Type,
    string WorkingModel,
    decimal Salary,
    string Currency,
    DateTime StartDate,
    DateTime EndDate,
    DateTime ApplicationDeadline
);
