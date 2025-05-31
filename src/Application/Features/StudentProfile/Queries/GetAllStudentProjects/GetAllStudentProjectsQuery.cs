using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Queries.GetAllStudentProjects;

public sealed record GetAllStudentProjectsQuery(Guid StudentId) : IQuery<IReadOnlyList<StudentProjectDto>>;