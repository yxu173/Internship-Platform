using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.DomainErrors;
using SharedKernel;
using Domain.Aggregates.Roadmaps;
using Domain.Enums;

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
            // If already enrolled and payment completed, return success
            if (existingEnrollment.PaymentStatus == PaymentStatus.Completed)
            {
                return Result.Success();
            }
            
            // If pending payment, also return success (let frontend initiate payment if needed)
            if (existingEnrollment.PaymentStatus == PaymentStatus.Pending)
            {
                return Result.Success(); 
            }
            
            // For any other status (failed, etc.), treat as duplicate enrollment
            return Result.Failure(RoadmapErrors.DuplicateEnrollment);
        }

        // Create new enrollment (status will be Pending by default)
        var enrollmentResult = Enrollment.Create(studentProfile.Id, request.RoadmapId);
        if (enrollmentResult.IsFailure)
        {
            return enrollmentResult; 
        }
        
        // For free roadmaps, mark enrollment as completed immediately
        if (!roadmap.IsPremium)
        {
            // Use "0" as transaction ID and 0.0 as amount for free roadmaps
            enrollmentResult.Value.CompletePayment("FREE_ENROLLMENT", 0.0m);
        }

        await _roadmapRepository.AddEnrollmentAsync(enrollmentResult.Value);

        return Result.Success();
    }
} 