using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyBasicInfo;

public sealed class UpdateCompanyBasicInfoCommandHandler : ICommandHandler<UpdateCompanyBasicInfoCommand, bool>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyBasicInfoCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<bool>> Handle(UpdateCompanyBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var result = await _companyRepository.UpdateBasicInfoAsync(request.UserId,
            request.Name,
            request.Industry,
            request.Description,
            request.WebsiteUrl,
            request.CompanySize
        );
        
        return result.IsFailure ? Result.Failure<bool>(result.Error) : Result.Success(true);
    }
}