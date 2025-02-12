using Application.Abstractions.Messaging;

namespace Application.Features.Internships.GetInternshipsByCompanyId;

public sealed record GetInternshipsByCompanyIdQuery(Guid UserId) : IQuery<IReadOnlyList<InternshipDto>>;