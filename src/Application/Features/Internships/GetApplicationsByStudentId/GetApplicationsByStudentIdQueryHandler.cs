using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.GetApplicationsByStudentId;

public sealed class GetApplicationsByStudentIdQueryHandler : IQueryHandler<GetApplicationsByStudentIdQuery, IReadOnlyList<StudentApplicationDto>>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IStudentRepository _studentRepository;

    public GetApplicationsByStudentIdQueryHandler(IInternshipRepository internshipRepository, IStudentRepository studentRepository)
    {
        _internshipRepository = internshipRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<IReadOnlyList<StudentApplicationDto>>> Handle(GetApplicationsByStudentIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var studentProfile = await _studentRepository.GetByIdAsync(request.UserId);
            if (studentProfile == null)
            {
                return Result.Failure<IReadOnlyList<StudentApplicationDto>>(Error.NotFound("Student.NotFound", "Student profile not found"));
            }

            var applications = await _internshipRepository.GetApplicationsByStudentIdAsync(studentProfile.Id);

            if (applications.Count == 0)
            {
                return Result.Success<IReadOnlyList<StudentApplicationDto>>(new List<StudentApplicationDto>());
            }

            var applicationDtos = applications.Select(a => new StudentApplicationDto(
                Id: a.Id,
                InternshipId: a.InternshipId,
                InternshipTitle: a.Internship.Title,
                CompanyName: a.Internship.CompanyProfile.CompanyName,
                Type: a.Internship.Type.ToString(),
                WorkingModel: a.Internship.WorkingModel.ToString(),
                Status: a.Status.ToString()
            )).ToList();

            return Result.Success<IReadOnlyList<StudentApplicationDto>>(applicationDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<StudentApplicationDto>>(Error.BadRequest("Failed to get student applications", ex.Message));
        }
    }
} 