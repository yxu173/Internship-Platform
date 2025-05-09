using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.StudentProfile.Queries.GetStudentBio;

public sealed class GetStudentBioQueryHandler : IQueryHandler<GetStudentBioQuery,string?>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentBioQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Result<string?>> Handle(GetStudentBioQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.UserId);
        
        return Result.Success(student.Bio);
    }
}