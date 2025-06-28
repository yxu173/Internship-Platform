using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.RemoveApplicationByCompany;

internal sealed class RemoveApplicationByCompanyCommandHandler
    : ICommandHandler<RemoveApplicationByCompanyCommand, bool>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly ICompanyRepository _companyRepository;

    public RemoveApplicationByCompanyCommandHandler(
        IInternshipRepository internshipRepository, 
        ICompanyRepository companyRepository)
    {
        _internshipRepository = internshipRepository;
        _companyRepository = companyRepository;
    }

    public async Task<Result<bool>> Handle(
        RemoveApplicationByCompanyCommand request,
        CancellationToken cancellationToken)
    {
        // Get the application
        var application = await _internshipRepository.GetApplicationByIdAsync(request.ApplicationId);
        if (application == null)
        {
            return Result.Failure<bool>(InternshipErrors.ApplicationNotFound);
        }

        // Get the internship to check ownership
        var internship = await _internshipRepository.GetByIdAsync(application.InternshipId);
        if (internship == null)
        {
            return Result.Failure<bool>(InternshipErrors.NotFound);
        }

        // Get the company profile to verify ownership
        var companyProfile = await _companyRepository.GetCompanyByIdAsync(request.CompanyUserId);
        if (companyProfile == null)
        {
            return Result.Failure<bool>(CompanyErrors.ProfileNotFound);
        }

        // Check if the company owns this internship
        if (internship.CompanyProfileId != companyProfile.Id)
        {
            return Result.Failure<bool>(InternshipErrors.NotInternshipOwner);
        }

        // Remove the application using domain method
        var removeResult = internship.RemoveApplication(application.Id);
        if (removeResult.IsFailure)
        {
            return Result.Failure<bool>(removeResult.Error);
        }

        // Update the internship in the repository
        await _internshipRepository.Update(internship);
        return Result.Success(true);
    }
} 