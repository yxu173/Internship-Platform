using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Application.Features.Identity.ChangePassword;

public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, bool>
{
    private readonly UserManager<User> _userManager;
    private readonly IApplicationDbContext _context;

    public ChangePasswordCommandHandler(UserManager<User> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
            return Result.Failure<bool>(Error.NotFound("ChangePassword.UserNotFound", "User not found"));

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Succeeded);
    }
}