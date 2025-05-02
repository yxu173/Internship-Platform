using System;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.DTOs;

public record RoadmapProgressDto(
    Guid RoadmapId,
    string Title,
    string Description,
    string Technology,
    Guid CompanyId,
    DateTime EnrolledAt,
    string PaymentStatus,
    double OverallCompletionPercentage,
    IReadOnlyList<RoadmapSectionProgressDto> Sections 
); 