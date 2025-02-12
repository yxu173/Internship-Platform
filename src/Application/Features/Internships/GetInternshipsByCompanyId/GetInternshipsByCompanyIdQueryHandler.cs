using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.GetInternshipsByCompanyId;

public sealed class GetInternshipsByCompanyIdQueryHandler :
    IQueryHandler<GetInternshipsByCompanyIdQuery,
        IReadOnlyList<InternshipDto>>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly ICompanyRepository _companyRepository;

    public GetInternshipsByCompanyIdQueryHandler(IInternshipRepository internshipRepository,
        ICompanyRepository companyRepository)
    {
        _internshipRepository = internshipRepository;
        _companyRepository = companyRepository;
    }

    public async Task<Result<IReadOnlyList<InternshipDto>>> Handle(
        GetInternshipsByCompanyIdQuery request,
        CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.UserId);
        if (company is null)
            return Result.Failure<IReadOnlyList<InternshipDto>>(CompanyErrors.ProfileNotFound);
        var internships = await _internshipRepository.GetByCompanyIdAsync(request.UserId);
        return internships.Select(i => new InternshipDto(i.Id,
            i.Title,
            i.Description,
            i.Duration.StartDate,
            i.Duration.EndDate,
            i.Type.ToString(),
            i.IsActive,
            i.CreatedAt)).ToList();
    }
}