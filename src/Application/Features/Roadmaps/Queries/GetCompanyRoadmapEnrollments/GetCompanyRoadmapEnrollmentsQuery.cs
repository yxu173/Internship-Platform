using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;
using System;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Queries.GetCompanyRoadmapEnrollments;

public record GetCompanyRoadmapEnrollmentsQuery(
    Guid CompanyId,
    Guid? RoadmapId = null 
) : IQuery<IReadOnlyList<CompanyRoadmapEnrollmentDto>>; 
