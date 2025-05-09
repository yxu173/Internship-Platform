using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Queries.GetStudentBio;

public sealed record GetStudentBioQuery(Guid UserId) : IQuery<string?>;