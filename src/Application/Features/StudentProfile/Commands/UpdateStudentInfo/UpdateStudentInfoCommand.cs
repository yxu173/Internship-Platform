using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.UpdateStudentInfo;

public sealed record UpdateStudentInfoCommand(
    Guid UserId,
    string FullName,
    string? PhoneNumber,
    string? Location,
    int Age,
    string Gender) : ICommand<bool>;