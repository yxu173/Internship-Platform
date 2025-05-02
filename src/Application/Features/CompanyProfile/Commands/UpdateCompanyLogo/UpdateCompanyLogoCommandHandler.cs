using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyLogo;

public sealed class UpdateCompanyLogoCommandHandler : ICommandHandler<UpdateCompanyLogoCommand, bool>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyLogoCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<bool>> Handle(UpdateCompanyLogoCommand request, CancellationToken cancellationToken)
    {
        var result = await _companyRepository
            .UpdateCompanyLogo(
                request.UserId,
                request.LogoUrl
            );

        if (result.IsFailure)
            return Result.Failure<bool>(result.Error);

        return result;
    }
}