using Application.Abstractions.Messaging;

namespace Application.Features.Internships.GetApplicationsByInternshipId;

public sealed record GetApplicationsByInternshipIdQuery(Guid InternshipId) : IQuery<IReadOnlyList<ApplicationDto>>;