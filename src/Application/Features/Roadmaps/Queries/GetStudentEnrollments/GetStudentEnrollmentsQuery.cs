using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using SharedKernel;
using System.Collections.Generic;

namespace Application.Features.Roadmaps.Queries.GetStudentEnrollments;

public record GetStudentEnrollmentsQuery(
    Guid UserId
) : IQuery<IReadOnlyList<EnrollmentSummaryDto>>;