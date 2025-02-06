using Application.Abstractions.Messaging;
using Application.Features.Profiles.GetStudentProfile;
using Domain.Aggregates.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Profiles.StudentProfile;

public sealed class GetStudentProfileQueryHandler 
    : IQueryHandler<GetStudentProfileQuery, StudentProfileDto>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentProfileQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<StudentProfileDto>> Handle(
        GetStudentProfileQuery request, 
        CancellationToken cancellationToken)
    {
        var profile = await _studentRepository.GetByUserIdAsync(request.UserId);
        
        if (profile is null)
            return Result.Failure<StudentProfileDto>(Domain.DomainErrors.StudentErrors.ProfileNotFound);

        return new StudentProfileDto(
            profile.FullName,
            profile.University.ToString(),
            profile.Faculty,
            profile.GraduationYear.Value,
            profile.Age,
            profile.Bio,
            profile.Skills
                .Select(ss => ss.Skill.Name)
                .ToList());
    }
}
