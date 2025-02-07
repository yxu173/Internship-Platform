using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Profiles.CreateCompanyProfile;

public sealed class CreateCompanyProfileCommandHandler : ICommandHandler<CreateCompanyProfileCommand, bool>
{
    private readonly ICompanyRepository _companyRepository;

    public CreateCompanyProfileCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<bool>> Handle(CreateCompanyProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _companyRepository.CreateAsync(
            request.UserId,
            request.CompanyName,
            request.TaxId,
            request.Governorate,
            request.Industry);

        if (result.IsFailure)
        {
            return Result.Failure<bool>(result.Error);
        }

        return Result.Success(true);
    }
}
