using Application.Abstractions.Messaging;
using Domain.Aggregates.Roadmaps;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.CreateRoadmap;

public sealed class CreateRoadmapCommandHandler : ICommandHandler<CreateRoadmapCommand, Guid>
{
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly ICompanyRepository _companyRepository;

    public CreateRoadmapCommandHandler(IRoadmapRepository roadmapRepository, ICompanyRepository companyRepository)
    {
        _roadmapRepository = roadmapRepository;
        _companyRepository = companyRepository;
    }

    public async Task<Result<Guid>> Handle(CreateRoadmapCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyByIdAsync(request.UserId);
        if (company == null)
            return Result.Failure<Guid>(CompanyErrors.ProfileNotFound);

        var roadmap = Roadmap.Create(request.Title
            , request.Description,
            request.Technology,
            request.IsPremium,
            request.Price,
            company.Id);

        await _roadmapRepository.AddAsync(roadmap.Value);

        return Result.Success(roadmap.Value.Id);
    }
}