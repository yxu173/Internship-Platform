namespace Web.Api.Contracts.Profile;

public record AddExperienceRequest(
    string JobTitle,
    string CompanyName,
    DateTime StartDate,
    DateTime EndDate);