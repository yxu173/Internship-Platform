using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;

namespace Application.Features.StudentProfile.Queries.GetAllStudentSkills;

public sealed record GetAllStudentSkillsQuery(Guid UserId) :IQuery<IReadOnlyList<Skill>>;