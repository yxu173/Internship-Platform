using System;

namespace Application.Features.Roadmaps.DTOs;

public record EnrollmentSummaryDto(
    Guid EnrollmentId,
    RoadmapInfoForEnrollmentDto Roadmap, 
    DateTime EnrolledAt,
    string PaymentStatus,
    double CompletionPercentage 
); 