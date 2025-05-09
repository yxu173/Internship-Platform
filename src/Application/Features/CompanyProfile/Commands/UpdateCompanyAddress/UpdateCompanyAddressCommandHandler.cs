using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.ValueObjects;
using SharedKernel;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyAddress;

public sealed class UpdateCompanyAddressCommandHandler : ICommandHandler<UpdateCompanyAddressCommand, bool>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyAddressCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<bool>> Handle(UpdateCompanyAddressCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyByIdAsync(request.UserId);

        company.UpdateAddress(request.Governorate, request.City);

        await _companyRepository.Update(company);

        return Result.Success(true);
    }
}