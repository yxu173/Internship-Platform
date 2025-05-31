using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Identity.Queries.GetUserType;

internal sealed class GetUserTypeQueryHandler : IQueryHandler<GetUserTypeQuery, UserTypeResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserTypeQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserTypeResponse>> Handle(GetUserTypeQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            return Result.Failure<UserTypeResponse>(Error.NotFound("GetUserType.UserNotFound", "User not found"));
        }

        string userType = "None";
        Guid? profileId = null;

        if (user.StudentProfile != null)
        {
            userType = "Student";
            profileId = user.StudentProfile.Id;
        }
        else if (user.CompanyProfile != null)
        {
            userType = "Company";
            profileId = user.CompanyProfile.Id;
        }

        return Result.Success(new UserTypeResponse(userType, profileId));
    }
} 