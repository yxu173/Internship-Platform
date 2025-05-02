using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.UpdateInternship;

public sealed class UpdateInternshipCommandHandler : ICommandHandler<UpdateInternshipCommand, bool>
{
    private readonly IInternshipRepository internshipRepository;

    public UpdateInternshipCommandHandler(IInternshipRepository internshipRepository)
    {
        this.internshipRepository = internshipRepository;
    }

    public async Task<Result<bool>> Handle(UpdateInternshipCommand request, CancellationToken cancellationToken)
    {
        var internship = await internshipRepository.GetByIdAsync(request.InternshipId);
        if (internship == null)
            return Result.Failure<bool>(InternshipErrors.InternshipClosed);

        var updateResult = internship.Update(
            request.Title,
            request.About,
            request.KeyResponsibilities,
            request.Requirements,
            request.Type,
            request.WorkingModel,
            request.Salary,
            request.Currency,
            request.ApplicationDeadline);

        if (updateResult.IsFailure)
            return Result.Failure<bool>(updateResult.Error);

        await internshipRepository.Update(internship);
        return Result.Success(true);
    }
}