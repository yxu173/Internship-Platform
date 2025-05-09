using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Queries.GetStudentEducation;

public sealed record GetStudentEducationQuery(Guid UserId) : IQuery<StudentEducationResponse>;