using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Application.Features.Identity.Login;

public sealed class LoginUserCommandHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenProvider tokenProvider)
    : ICommandHandler<LoginUserCommand, string>
{
    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure<string>(UserErrors.NotFoundByEmail);
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        
        if (!result.Succeeded)
        {
            return Result.Failure<string>(UserErrors.EmailNotUnique);
        }

        

        var token = tokenProvider.Create(user);

        return Result.Success(token);
    }
}