using Application.Abstractions.Messaging;
using Domain.Enums;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Internships.RejectApplication;

public sealed class RejectApplicationCommandHandler : ICommandHandler<RejectApplicationCommand, bool>
{
    private readonly IInternshipRepository _internshipRepository;

    public RejectApplicationCommandHandler(IInternshipRepository internshipRepository)
    {
        _internshipRepository = internshipRepository;
    }

    public Task<Result<bool>> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}