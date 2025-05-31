using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompanyInfo;

public sealed class GetCompanyInfoQueryHandler : IQueryHandler<GetCompanyInfoQuery, CompanyInfoResponse>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyInfoQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompanyInfoResponse>> Handle(GetCompanyInfoQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyByIdAsync(request.UserId);

        var response = new CompanyInfoResponse(company.CompanyName, company.Description);

        return Result.Success(response);
    }
}