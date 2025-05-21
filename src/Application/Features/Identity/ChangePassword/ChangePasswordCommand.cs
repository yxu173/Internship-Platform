using Application.Abstractions.Messaging;

namespace Application.Features.Identity.ChangePassword;

public sealed record ChangePasswordCommand(Guid UserId, string OldPassword, string NewPassword) : ICommand<bool>;