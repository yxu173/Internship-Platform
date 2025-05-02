using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Queries.GetAllStudentExperiences;

public sealed class
    GetAllStudentExperiencesQueryHandler : IQueryHandler<GetAllStudentExperiencesQuery,
    IReadOnlyList<StudentExperienceDto>>
{
    private readonly IStudentRepository _studentRepository;

    public GetAllStudentExperiencesQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<IReadOnlyList<StudentExperienceDto>>> Handle(GetAllStudentExperiencesQuery request,
        CancellationToken cancellationToken)
    {
        var experiences = await _studentRepository
            .GetAllStudentExperiences(request.StudentId);
        var result = experiences.Select(e => new StudentExperienceDto(
            e.Id,
            e.JobTitle,
            e.CompanyName,
            e.DateRange.StartDate,
            e.DateRange.EndDate
        )).ToList();
        return Result.Success<IReadOnlyList<StudentExperienceDto>>(result);
    }
}