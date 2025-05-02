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
        var internships = await _internshipRepository.GetByCompanyIdAsync(request.UserId);
        return Result.Success<IReadOnlyList<InternshipDto>>(
            internships.Select(i => new InternshipDto
                (
                    i.Id,
                    i.Title,
                    i.About,
                    i.KeyResponsibilities,
                    i.Requirements,
                    i.Duration.StartDate,
                    i.Duration.EndDate,
                    i.Type.ToString(),
                    i.WorkingModel.ToString(),
                    i.Salary.Amount,
                    i.Salary.Currency,
                    i.IsActive,
                    i.CreatedAt))
                .ToList());
    }
}