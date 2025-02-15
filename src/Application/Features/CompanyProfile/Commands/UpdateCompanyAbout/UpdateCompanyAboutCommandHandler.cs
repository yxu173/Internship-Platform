using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.CompanyProfile.Commands.UpdateCompanyAbout;

public sealed class UpdateCompanyAboutCommandHandler : ICommandHandler<UpdateCompanyAboutCommand, bool>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyAboutCommandHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<bool>> Handle(UpdateCompanyAboutCommand request, CancellationToken cancellationToken)
    {
        var result = await _companyRepository
            .UpdateCompanyAbout(
                request.UserId,
                request.About,
                request.Mission,
                request.Vision
            );
        
        if (result.IsFailure)
            return Result.Failure<bool>(result.Error);
        
        return result;
    }
}