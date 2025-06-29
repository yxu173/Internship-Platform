using Application.Abstractions.Messaging;

namespace Application.Features.Internships.GetApplicationsByStudentId;

public sealed record GetApplicationsByStudentIdQuery(Guid UserId) : IQuery<IReadOnlyList<StudentApplicationDto>>; 