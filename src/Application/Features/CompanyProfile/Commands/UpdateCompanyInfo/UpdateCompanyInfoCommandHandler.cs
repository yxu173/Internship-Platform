using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyInfo;

public sealed class UpdateCompanyInfoCommandHandler : ICommandHandler<UpdateCompanyInfoCommand, bool>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyInfoCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<bool>> Handle(UpdateCompanyInfoCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyByIdAsync(request.UserId);

        company.UpdateInfo(request.Name, request.Description);

        await _companyRepository.Update(company);
        
        return Result.Success(true);
    }
}