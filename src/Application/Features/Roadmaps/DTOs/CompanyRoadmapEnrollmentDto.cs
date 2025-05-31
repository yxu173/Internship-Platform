using System;

namespace Application.Features.Roadmaps.DTOs;

public record CompanyRoadmapEnrollmentDto(
    Guid EnrollmentId,
    Guid RoadmapId,
    StudentInfoForCompanyDto Student,
    DateTime EnrolledAt,
    string PaymentStatus,
    double CompletionPercentage
); 