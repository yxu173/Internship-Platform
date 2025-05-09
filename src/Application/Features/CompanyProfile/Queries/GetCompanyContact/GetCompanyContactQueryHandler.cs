using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Queries.GetCompanyContact;

public sealed class GetCompanyContactQueryHandler : IQueryHandler<GetCompanyContactQuery, CompanyContactResponse>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;

    public GetCompanyContactQueryHandler(ICompanyRepository companyRepository, IUserRepository userRepository)
    {
        _companyRepository = companyRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<CompanyContactResponse>> Handle(GetCompanyContactQuery request,
        CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyByIdAsync(request.UserId);
        var user = await _userRepository.GetByIdAsync(request.UserId);
        var response =
            new CompanyContactResponse(user.Email, company.Address.Governorate.ToString(), company.Address.City);
        return Result.Success(response);
    }
}