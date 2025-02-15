using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Queries.GetAllStudentProfile;

public record GetAllStudentProfileQuery(Guid StudentId) : IQuery<CompleteStudentProfileResponse>;