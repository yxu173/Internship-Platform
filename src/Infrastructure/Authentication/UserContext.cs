using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new ApplicationException("User context is unavailable");

    public Guid? StudentId => GetClaimValueAsGuid("student_id");
    public Guid? CompanyId => GetClaimValueAsGuid("company_id");

    private Guid? GetClaimValueAsGuid(string claimType)
    {
        var claimValue = _httpContextAccessor?
            .HttpContext?
            .User?
            .FindFirst(claimType)?
            .Value;

        if (Guid.TryParse(claimValue, out var guidValue))
        {
            return guidValue;
        }
        
        return null;
    }
}