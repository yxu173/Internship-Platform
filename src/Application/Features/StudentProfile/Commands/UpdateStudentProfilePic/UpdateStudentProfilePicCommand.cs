using Application.Abstractions.Messaging;

namespace Application.Features.StudentProfile.Commands.UpdateStudentProfilePic;

public sealed record UpdateStudentProfilePicCommand(Guid UserId, string ProfilePicUrl) : ICommand<bool>;