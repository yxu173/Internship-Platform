using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Queries.GetStudentProfile;

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
            profile.Id,
            profile.UserId,
            profile.FullName,
            profile.PhoneNumber.Value,
            profile.Location,
            profile.Age,
            profile.Gender.ToString());
    }
}