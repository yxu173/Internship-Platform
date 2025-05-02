using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Features.Notifications.Commands.SendNotification;
using Domain.Aggregates.Users;
using Domain.DomainErrors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Application.Features.Identity.Login;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, string>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly ISender _sender;

    public LoginUserCommandHandler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenProvider tokenProvider,
        ISender sender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProvider = tokenProvider;
        _sender = sender;
    }

    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure<string>(UserErrors.InvalidCredentials);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Result.Failure<string>(UserErrors.InvalidCredentials);
        }

        // Send welcome back notification
        var notificationCommand = new SendNotificationCommand(
            user.Id,
            "Welcome Back!",
            $"Welcome back to the platform, {user.UserName}!",
            "Login",
            user.Id);

        await _sender.Send(notificationCommand, cancellationToken);

        return Result.Success(_tokenProvider.Create(user));
    }
}