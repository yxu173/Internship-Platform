using Application.Abstractions.Messaging;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.RemoveInternship;

public sealed class RemoveInternshipCommandHandler : ICommandHandler<RemoveInternshipCommand, bool>
{
        private readonly IInternshipRepository internshipRepository;

    public RemoveInternshipCommandHandler(IInternshipRepository internshipRepository)
    {
        this.internshipRepository = internshipRepository;
    }

    public async Task<Result<bool>> Handle(RemoveInternshipCommand request, CancellationToken cancellationToken)
    {
        var internship = await internshipRepository.GetByIdAsync(request.InternshipId);
        if (internship == null)
            return Result.Failure<bool>(InternshipErrors.InternshipClosed);

        await internshipRepository.Delete(internship);
        return Result.Success(true);
    }
}