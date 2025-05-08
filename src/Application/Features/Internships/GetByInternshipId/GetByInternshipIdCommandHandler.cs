using Application.Abstractions.Messaging;
using Application.Features.Internships.GetInternshipsByCompanyId;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.GetByInternshipId;

public sealed class GetByInternshipIdCommandHandler : ICommandHandler<GetByInternshipIdCommand, InternshipResponse>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IInternshipBookmarkRepository _internshipBookmarkRepository;

    public GetByInternshipIdCommandHandler(IInternshipRepository internshipRepository,
        IInternshipBookmarkRepository internshipBookmarkRepository)
    {
        _internshipRepository = internshipRepository;
        _internshipBookmarkRepository = internshipBookmarkRepository;
    }

    public async Task<Result<InternshipResponse>> Handle(GetByInternshipIdCommand request,
        CancellationToken cancellationToken)
    {
        var internship = await _internshipRepository.GetByInternshipIdWithCompanyAsync(request.InternshipId);
        var bookmarks =
            await _internshipBookmarkRepository.GetByUserIdAsync(request.UserId, cancellationToken: cancellationToken);
        var internshipBookmark =
            bookmarks
                .FirstOrDefault(internshipBookmark => internshipBookmark.InternshipId == request.InternshipId);

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
            internship.CompanyProfile.About.Mission,
            internship.CompanyProfile.About.Vision,
            internship.CompanyProfile.About.About,
            internshipBookmark != null,
            internship.CreatedAt
        );
        return Result.Success(response);
    }
}