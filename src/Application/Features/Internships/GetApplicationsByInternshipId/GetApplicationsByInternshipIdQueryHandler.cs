using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Internships.GetApplicationsByInternshipId;

public sealed class GetApplicationsByInternshipIdQueryHandler : IQueryHandler<GetApplicationsByInternshipIdQuery, IReadOnlyList<ApplicationDto>>
{
    public Task<Result<IReadOnlyList<ApplicationDto>>> Handle(GetApplicationsByInternshipIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}