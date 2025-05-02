using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Commands.CreateCompanyProfile;

public sealed class CreateCompanyProfileCommandHandler : ICommandHandler<CreateCompanyProfileCommand, Guid>
{
    private readonly ICompanyRepository _companyRepository;

    public CreateCompanyProfileCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<Guid>> Handle(CreateCompanyProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _companyRepository.CreateAsync(
            request.UserId,
            request.CompanyName,
            request.TaxId,
            request.Governorate,
            request.City,
            request.Street,
            request.Industry);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        return Result.Success(result.Value.Id);
    }
}