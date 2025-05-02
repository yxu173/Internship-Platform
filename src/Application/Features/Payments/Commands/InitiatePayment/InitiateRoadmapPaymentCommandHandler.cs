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

        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.RoadmapId == request.RoadmapId && e.StudentId == studentProfile.Id);

        if (existingEnrollment == null)
        {
            var enrollmentResult = Enrollment.Create(studentProfile.Id, request.RoadmapId);
            if (enrollmentResult.IsFailure)
            {
                return Result.Failure<PaymentInitiationResponse>(
                    Error.BadRequest("Enrollment.Failed", enrollmentResult.Error.Description));
            }

            _context.Enrollments.Add(enrollmentResult.Value);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else if (existingEnrollment.PaymentStatus == Domain.ValueObjects.PaymentStatus.Completed)
        {
            return Result.Failure<PaymentInitiationResponse>(
                Error.Conflict("Enrollment.AlreadyCompleted", "You are already enrolled in this roadmap."));
        }

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