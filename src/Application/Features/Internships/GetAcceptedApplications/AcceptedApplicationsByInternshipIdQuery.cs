using Application.Abstractions.Messaging;
using Application.Features.Internships.GetApplicationsByInternshipId;

namespace Application.Features.Internships.GetAcceptedApplications;

public sealed record AcceptedApplicationsByInternshipIdQuery(Guid InternshipId) : IQuery<IReadOnlyList<ApplicationDto>>; 