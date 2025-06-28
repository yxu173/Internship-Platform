using MediatR;
using SharedKernel;
using Application.Common.Interfaces;
using Domain.Repositories;
using Domain.Aggregates.Roadmaps;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Enums;

namespace Application.Features.Payments.Commands.InitiatePayment;

public class InitiateRoadmapPaymentCommandHandler
    : ICommandHandler<InitiateRoadmapPaymentCommand, PaymentInitiationResponse>
{
    private readonly IPaymentService _paymentService;
    private readonly IApplicationDbContext _context;
    private readonly IStudentRepository _studentRepository;

    public InitiateRoadmapPaymentCommandHandler(
        IPaymentService paymentService,
        IApplicationDbContext context,
        IStudentRepository studentRepository)
    {
        _paymentService = paymentService;
        _context = context;
        _studentRepository = studentRepository;
    }

    public async Task<Result<PaymentInitiationResponse>> Handle(
        InitiateRoadmapPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var roadmap = await _context.Roadmaps
            .FirstOrDefaultAsync(r => r.Id == request.RoadmapId, cancellationToken);

        if (roadmap == null)
        {
            return Result.Failure<PaymentInitiationResponse>(
                Error.NotFound("Roadmap.NotFound", $"Roadmap with ID {request.RoadmapId} not found."));
        }

        if (!roadmap.IsPremium || roadmap.Price <= 0)
        {
            return Result.Failure<PaymentInitiationResponse>(
                Error.BadRequest("Roadmap.NotPremium", "Roadmap is not premium or has no price."));
        }

        var studentProfile = await _studentRepository.GetByUserIdAsync(request.StudentId);
        if (studentProfile == null)
        {
            return Result.Failure<PaymentInitiationResponse>(
                Error.NotFound("Student.NotFound", $"Student profile not found for user ID {request.StudentId}."));
        }

        // Check if already enrolled and payment completed
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.RoadmapId == request.RoadmapId && e.StudentId == studentProfile.Id);

        if (existingEnrollment != null && existingEnrollment.PaymentStatus == PaymentStatus.Completed)
        {
            return Result.Failure<PaymentInitiationResponse>(
                Error.Conflict("Enrollment.AlreadyCompleted", "You are already enrolled in this roadmap."));
        }

        // Create a temporary payment tracking record
        // This will be used to create the enrollment during the redirect
        var paymentTracking = new PaymentTrackingInfo
        {
            RoadmapId = request.RoadmapId,
            StudentId = studentProfile.Id,
            UserId = request.StudentId,
            CreatedAt = DateTime.UtcNow
        };

        // Store in a simple way - for now, we'll use a static dictionary
        // In production, this should be stored in a database or cache
        PaymentTrackingStore.StorePayment(paymentTracking);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);

        if (user == null)
        {
            return Result.Failure<PaymentInitiationResponse>(
                Error.NotFound("User.NotFound", $"User with ID {request.StudentId} not found."));
        }

        var email = user.Email ?? "not_provided@example.com";
        var fullName = user.UserName ?? "User";
        var firstName = fullName.Split(' ')[0];
        var lastName = fullName.Contains(' ') ? fullName.Substring(fullName.IndexOf(' ') + 1) : "User";
        var phoneNumber = user.PhoneNumber ?? "NA";

        var result = await _paymentService.InitiatePaymentAsync(
            request.RoadmapId,
            roadmap.Title,
            roadmap.Price ?? 0m,
            "EGP",
            request.StudentId,
            email,
            firstName,
            lastName,
            phoneNumber
        );

        return result;
    }
}