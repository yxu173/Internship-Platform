using Microsoft.AspNetCore.Identity;

namespace Domain.Users;

public sealed class User : IdentityUser<Guid>
{
    private User()
    {
    }

    public string FullName { get; private set; }
    public static User Create(string email, string userName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = userName
        };
    }
}
