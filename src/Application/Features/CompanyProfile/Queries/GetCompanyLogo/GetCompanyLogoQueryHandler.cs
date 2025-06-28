using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompanyLogo;

public sealed class GetCompanyLogoQueryHandler : IQueryHandler<GetCompanyLogoQuery, string?>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyLogoQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<string?>> Handle(GetCompanyLogoQuery request, CancellationToken cancellationToken)
    {
        var result = await _companyRepository.GetByUserIdAsync<string?>(
            request.UserId,
            x => x.LogoUrl
        );

        if (result.IsFailure)
            return Result.Failure<string?>(result.Error);

        // If no company profile is found, return null (which is valid for string?)
        if (result.Value == null)
            return Result.Success<string?>(null);

        return Result.Success(result.Value);
    }
}