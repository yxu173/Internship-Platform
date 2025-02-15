using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Aggregates.Internships;
using Domain.Aggregates.Users;
using Domain.Repositories;
using Domain.ValueObjects;
using SharedKernel;

namespace Application.Features.Internships.CreateInternship;
public sealed class CreateInternshipCommandHandler : ICommandHandler<CreateInternshipCommand, Guid>
{
    private readonly IInternshipRepository internshipRepository;
    private readonly ICompanyRepository companyRepository;

    public CreateInternshipCommandHandler(IInternshipRepository internshipRepository,ICompanyRepository companyRepository)
    {
        this.internshipRepository = internshipRepository;
        this.companyRepository = companyRepository;
    }

    public async Task<Result<Guid>> Handle(CreateInternshipCommand request, CancellationToken cancellationToken)
    {
        var companyProfile = await companyRepository.GetCompanyByIdAsync(request.UserId);
        var Duration = DateRange.Create(request.StartDate,request.EndDate);
         var internshipResult = Internship.Create(
            request.Title,
            request.About,
            request.KeyResponsibilities,
            request.Requirements,
            companyProfile.Id,
            request.Type,
            request.WorkingModel,
            request.Salary,
            request.Currency,
            Duration.Value,
            request.ApplicationDeadline);

        if (internshipResult.IsFailure)
            return Result.Failure<Guid>(internshipResult.Error);

        await internshipRepository.AddAsync(internshipResult.Value);
        return Result.Success(internshipResult.Value.Id);
    }
}
