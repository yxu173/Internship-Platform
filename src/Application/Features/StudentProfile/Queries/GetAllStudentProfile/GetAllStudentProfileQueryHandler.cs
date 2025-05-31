using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Queries.GetAllStudentProfile;

public sealed class
    GetAllStudentProfileQueryHandler : IQueryHandler<GetAllStudentProfileQuery, CompleteStudentProfileResponse>
{
    private readonly IStudentRepository _studentRepository;

    public GetAllStudentProfileQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<CompleteStudentProfileResponse>> Handle(GetAllStudentProfileQuery request,
        CancellationToken cancellationToken)
    {
        var studentProfile = await _studentRepository.GetCompleteStudentProfile(request.StudentId);

        if (studentProfile is null)
            return Result.Failure<CompleteStudentProfileResponse>(StudentErrors.ProfileNotFound);

        return Result.Success(new CompleteStudentProfileResponse(
            Id: studentProfile.Id,
            Bio: studentProfile.Bio,
            ProfilePictureUrl: string.IsNullOrEmpty(studentProfile.ProfilePictureUrl)
                ? "/uploads/profile-pics/default-profile.png"
                : studentProfile.ProfilePictureUrl,
            ResumeUrl: string.IsNullOrEmpty(studentProfile.ResumeUrl) ? string.Empty : studentProfile.ResumeUrl,
            BasicInfo: new BasicInfoResponse(
                FullName: studentProfile.FullName,
                Age: studentProfile.Age,
                Gender: studentProfile.Gender.ToString(),
                PhoneNumber: studentProfile.PhoneNumber.Value,
                Location: studentProfile.Location,
                Email: " "
            ),
            Education: new EducationResponse(
                studentProfile.University.ToString(),
                studentProfile.Faculty,
                studentProfile.GraduationYear.Value,
                studentProfile.EnrollmentYear.Value,
                studentProfile.Role
            ),
            Skills: studentProfile.Skills
                .Select(ss => new SkillResponse(
                    Id: ss.SkillId,
                    Name: ss.Skill.Name
                ))
                .ToList(),
            Experiences: studentProfile.Experiences
                .Select(exp => new ExperienceResponse(
                    Id: exp.Id,
                    JobTitle: exp.JobTitle,
                    CompanyName: exp.CompanyName,
                    StartDate: exp.DateRange.StartDate,
                    EndDate: exp.DateRange.EndDate
                ))
                .ToList(),
            Projects: studentProfile.Projects
                .Select(p => new ProjectResponse(
                    Id: p.Id,
                    ProjectName: p.ProjectName,
                    Description: p.Description,
                    ProjectUrl: p.ProjectUrl ?? string.Empty
                ))
                .ToList()
        ));
    }
}