using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Features.Notifications.Commands.SendNotification;
using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Application.Features.Identity.Login;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly ISender _sender;

    public LoginUserCommandHandler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenProvider tokenProvider,
        ISender sender, IUserRepository userRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProvider = tokenProvider;
        _sender = sender;
        _userRepository = userRepository;
    }

    public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
        }

        var notificationCommand = new SendNotificationCommand(
            user.Id,
            "Welcome Back!",
            $"Welcome back to the platform, {user.UserName}!",
            "Login",
            user.Id);

        await _sender.Send(notificationCommand, cancellationToken);

        var userProfile = await _userRepository.GetByIdAsync(user.Id);
        string userType = "None";
        Guid? profileId = null;

        if (userProfile.StudentProfile != null)
        {
            userType = "Student";
            profileId = userProfile.StudentProfile.Id;
        }
        else if (userProfile.CompanyProfile != null)
        {
            userType = "Company";
            profileId = userProfile.CompanyProfile.Id;
        }

        string token = _tokenProvider.Create(user);
        
        var response = new LoginResponse(token, userType, profileId);

        return Result.Success(response);
    }
}