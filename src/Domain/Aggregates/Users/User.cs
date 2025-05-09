using Domain.Aggregates.Profiles;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using SharedKernel;


namespace Domain.Aggregates.Users;

public sealed class User : IdentityUser<Guid>
{
    public string? LinkedInUrl { get; private set; }
    public string? GitHubUrl { get; private set; }
    public StudentProfile? StudentProfile { get; private set; }
    public CompanyProfile? CompanyProfile { get; private set; }
    public bool ProfileComplete { get; private set; } = false;

    private User()
    {
    }

    private User(string email, string userName)
    {
        Email = email;
        UserName = userName;
    }


    public static Result<User> Create(string email, string userName)
    {
        try
        {
            return Result.Success(new User(email.Trim(), userName.Trim()));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<User>(Error.Validation("User.Create", ex.Message));
        }
    }

    public Result<StudentProfile> CreateStudentProfile(
        string fullName,
        string university,
        string faculty,
        int graduationYear,
        int enrollmentYear,
        int age,
        string gender,
        string phoneNumber,
        string? bio,
        string? profilePictureUrl)
    {
        var profileResult = StudentProfile.Create(
            fullName, university, faculty, graduationYear, enrollmentYear,
            age, gender, phoneNumber, bio, profilePictureUrl);

        if (profileResult.IsFailure)
            return profileResult;

        StudentProfile = profileResult.Value;
        ProfileComplete = true;
        return Result.Success(profileResult.Value);
    }


    public Result CreateCompanyProfile(
        string companyName,
        string taxId,
        string governorate,
        string city,
        string industry)
    {
        var profileResult = CompanyProfile.Create(companyName, taxId, governorate, city, industry);
        if (profileResult.IsFailure)
            return profileResult;

        CompanyProfile = profileResult.Value;
        ProfileComplete = true;
        return Result.Success();
    }

    public void UpdateSocialLinks(string? linkedInUrl, string? gitHubUrl)
    {
        LinkedInUrl = linkedInUrl?.Trim();
        GitHubUrl = gitHubUrl?.Trim();
    }
}