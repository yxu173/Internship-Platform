using Application.Abstractions.Messaging;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Features.Profiles.StudentProfile;


public sealed record CreateStudentProfileCommand(
Guid UserId,
string FullName,
string University,
string Faculty,
int GraduationYear,
int Age,
string Gender,
string PhoneNumber
) : ICommand<bool>;