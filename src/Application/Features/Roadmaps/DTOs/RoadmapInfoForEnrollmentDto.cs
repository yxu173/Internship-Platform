namespace Application.Features.Roadmaps.DTOs;

public record RoadmapInfoForEnrollmentDto(
    Guid Id,
    string Title,
    string Technology,
    Guid CompanyId 
); 