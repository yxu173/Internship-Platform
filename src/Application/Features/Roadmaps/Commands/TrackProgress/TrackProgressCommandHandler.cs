using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps;

namespace Application.Features.Roadmaps.Commands.TrackProgress;

internal sealed class TrackProgressCommandHandler : ICommandHandler<TrackProgressCommand>
{
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IStudentRepository _studentRepository;

    public TrackProgressCommandHandler(IRoadmapRepository roadmapRepository, IStudentRepository studentRepository)
    {
        _roadmapRepository = roadmapRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result> Handle(TrackProgressCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        var enrollment = await _roadmapRepository.GetEnrollmentAsync(student.Id, request.RoadmapId);
        if (enrollment is null)
        {
            return Result.Failure(RoadmapErrors.EnrollmentNotFound);
        }

        var existingProgress = await _roadmapRepository.GetProgressAsync(enrollment.Id, request.ItemId);
        if (existingProgress is not null)
        {
            return Result.Failure(RoadmapErrors.DuplicateProgress);
        }
        
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);
        if (roadmap is null || !roadmap.Sections.Any(s => s.Items.Any(i => i.Id == request.ItemId)))
        {
            return Result.Failure(RoadmapErrors.ItemNotFound); 
        }

        var progress = new ResourceProgress(enrollment.Id, request.ItemId);

        await _roadmapRepository.TrackProgressAsync(progress);

        return Result.Success();
    }
}