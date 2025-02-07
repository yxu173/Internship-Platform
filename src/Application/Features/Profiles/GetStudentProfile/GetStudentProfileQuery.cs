using Application.Abstractions.Messaging;
using Application.Features.Profiles.GetStudentProfile;

namespace Application.Features.Profiles.StudentProfile;

public sealed record GetStudentProfileQuery(Guid UserId) 
    : IQuery<StudentProfileDto>;