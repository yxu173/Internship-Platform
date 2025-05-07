using Application.Abstractions.Messaging;
using Application.Features.Internships.GetInternshipsByCompanyId;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.GetByInternshipId;

public sealed class GetByInternshipIdCommandHandler : ICommandHandler<GetByInternshipIdCommand, InternshipResponse>
{
    private readonly IInternshipRepository _internshipRepository;

    public GetByInternshipIdCommandHandler(IInternshipRepository internshipRepository)
    {
        _internshipRepository = internshipRepository;
    }

    public async Task<Result<InternshipResponse>> Handle(GetByInternshipIdCommand request, CancellationToken cancellationToken)
    {
        var internship = await _internshipRepository.GetByInternshipIdWithCompanyAsync(request.InternshipId);
        var response = new InternshipResponse
        (
            internship.Id,
            internship.CompanyProfileId,
            internship.CompanyProfile.CompanyName,
            internship.CompanyProfile.LogoUrl,
            internship.CompanyProfile.Address.Governorate.ToString(),
            internship.CompanyProfile.Address.City,
            internship.Title,
            internship.About,
            internship.KeyResponsibilities,
            internship.Requirements,
            internship.Duration.StartDate,
            internship.Duration.EndDate,
            internship.Type.ToString(),
            internship.WorkingModel.ToString(),
            internship.Salary.Amount,
            internship.Salary.Currency,
            internship.IsActive,
            internship.CreatedAt
        );
        return Result.Success(response);
    }
}