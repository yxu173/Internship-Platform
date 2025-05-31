using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Queries.GetStudentProfile;

public sealed record GetStudentProfileQuery(Guid UserId) 
    : IQuery<StudentProfileDto>;