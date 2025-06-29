using Domain.Enums;

namespace Application.Features.Internships.GetApplicationsByStudentId;

public sealed record StudentApplicationDto(
    Guid Id,
    Guid InternshipId,
    string InternshipTitle,
    string CompanyName,
    string Type,
    string WorkingModel,
    string Status); 