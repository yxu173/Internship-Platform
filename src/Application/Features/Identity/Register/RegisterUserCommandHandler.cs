using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Aggregates.Users;
using Domain.DomainErrors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Identity.Register;

internal sealed class RegisterUserCommandHandler(
    IApplicationDbContext context,
    UserManager<User> userManager,
    ITokenProvider tokenProvider)
    : ICommandHandler<RegisterUserCommand, string>
{
    public async Task<Result<string>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken) != null)
        {
            return Result.Failure<string>(UserErrors.EmailNotUnique);
        }

        var user = User.Create( command.Email, command.UserName);
        if (user.IsFailure)
        {
            return Result.Failure<string>(user.Error);
        }

        var result = await userManager.CreateAsync(user.Value, command.Password);
        var addRoleResult = await userManager.AddToRoleAsync(user.Value, Roles.Basic.ToString());
        if (!result.Succeeded)
        {
            return Result.Failure<string>(UserErrors.RegisterUserError);
        }

        var token = tokenProvider.Create(user.Value);   

        return Result.Success(token);
    }
}