using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Queries.GetStudentEducation;

public sealed class GetStudentEducationQueryHandler : IQueryHandler<GetStudentEducationQuery, StudentEducationResponse>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentEducationQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<StudentEducationResponse>> Handle(GetStudentEducationQuery request,
        CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);

        var response = new StudentEducationResponse(
            student.University.ToString(),
            student.Faculty,
            student.GraduationYear.Value,
            student.EnrollmentYear.Value,
            student.Role);
        
        return Result.Success(response);
    }
}