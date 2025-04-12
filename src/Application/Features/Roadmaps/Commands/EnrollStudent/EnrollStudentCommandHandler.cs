using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps;

namespace Application.Features.Roadmaps.Commands.EnrollStudent;

internal sealed class EnrollStudentCommandHandler : ICommandHandler<EnrollStudentCommand>
{
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IStudentRepository _studentRepository;

    public EnrollStudentCommandHandler(IRoadmapRepository roadmapRepository, IStudentRepository studentRepository)
    {
        _roadmapRepository = roadmapRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId);
        if (roadmap is null)
        {
            return Result.Failure(RoadmapErrors.RoadmapNotFound);
        }

        var studentProfile = await _studentRepository.GetByUserIdAsync(request.UserId);
        if (studentProfile is null)
        {
            return Result.Failure(StudentErrors.ProfileNotFound);
        }

        var existingEnrollment = await _roadmapRepository.GetEnrollmentAsync(studentProfile.Id, request.RoadmapId);
        if (existingEnrollment is not null)
        {
            return Result.Failure(RoadmapErrors.DuplicateEnrollment);
        }

        if (roadmap.IsPremium)
        {
            return Result.Failure(RoadmapErrors.PremiumEnrollmentRequiresPayment);
        }

        var enrollmentResult = Enrollment.Create(studentProfile.Id, request.RoadmapId);
        if (enrollmentResult.IsFailure)
        {
            return enrollmentResult; 
        }

        await _roadmapRepository.AddEnrollmentAsync(enrollmentResult.Value);

        return Result.Success();
    }
} 