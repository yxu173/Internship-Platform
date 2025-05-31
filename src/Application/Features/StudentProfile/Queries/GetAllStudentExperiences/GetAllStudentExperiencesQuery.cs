using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Queries.GetAllStudentExperiences;

public record GetAllStudentExperiencesQuery(Guid StudentId) : IQuery<IReadOnlyList<StudentExperienceDto>>;