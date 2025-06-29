using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Aggregates.Roadmaps;
using Domain.DomainErrors;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Application.Features.Payments.Commands.InitiatePayment;

namespace Application.Features.Payments.Commands.ProcessRedirect;

public sealed class ProcessPaymentRedirectCommandHandler : ICommandHandler<ProcessPaymentRedirectCommand,Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<ProcessPaymentRedirectCommandHandler> _logger;

    public ProcessPaymentRedirectCommandHandler(
        IApplicationDbContext dbContext,
        ILogger<ProcessPaymentRedirectCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(ProcessPaymentRedirectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment redirect for OrderId: {OrderId}, TransactionId: {TransactionId}",
            request.OrderId, request.TransactionId);

        try
        {
            if (string.IsNullOrEmpty(request.OrderId))
            {
                _logger.LogWarning("Empty OrderId in payment redirect");
                return Result.Failure<Guid>(Error.BadRequest("Payment.InvalidOrderId", "Invalid order ID"));
            }

            // Try to find the most recent payment tracking information
            // Since we don't have the roadmap and user IDs from the redirect,
            // we'll look for the most recent payment tracking record
            var paymentTracking = PaymentTrackingStore.GetMostRecentPayment();

            if (paymentTracking == null)
            {
                _logger.LogWarning("No payment tracking information found for redirect. OrderId: {OrderId}, TransactionId: {TransactionId}",
                    request.OrderId, request.TransactionId);
                return Result.Failure<Guid>(Error.NotFound("Payment.NoTrackingInfo", 
                    "No payment tracking information found for this payment."));
            }

            // Check if enrollment already exists
            var existingEnrollment = await _dbContext.Enrollments
                .FirstOrDefaultAsync(e => e.RoadmapId == paymentTracking.RoadmapId && e.StudentId == paymentTracking.StudentId, cancellationToken);

            if (existingEnrollment != null && existingEnrollment.PaymentStatus == PaymentStatus.Completed)
            {
                _logger.LogInformation("Enrollment already completed for Student {StudentId} in Roadmap {RoadmapId}",
                    paymentTracking.StudentId, paymentTracking.RoadmapId);
                PaymentTrackingStore.RemovePayment(paymentTracking.RoadmapId, paymentTracking.UserId);
                return Result.Success(existingEnrollment.RoadmapId);
            }

            // Create or complete the enrollment
            if (existingEnrollment == null)
            {
                var enrollmentResult = Enrollment.Create(paymentTracking.StudentId, paymentTracking.RoadmapId);
                if (enrollmentResult.IsFailure)
                {
                    _logger.LogError("Failed to create enrollment for Student {StudentId} in Roadmap {RoadmapId}",
                        paymentTracking.StudentId, paymentTracking.RoadmapId);
                    return Result.Failure<Guid>(Error.Problem("Payment.EnrollmentCreation",
                        "Failed to create enrollment after successful payment"));
                }

                existingEnrollment = enrollmentResult.Value;
                _dbContext.Enrollments.Add(existingEnrollment);
            }

            // Complete the payment
            existingEnrollment.CompletePayment(
                request.TransactionId,
                100.0m // Default amount - ideally should be extracted from the payment response
            );

            _logger.LogInformation("Completing payment for Enrollment {EnrollmentId}, Roadmap: {RoadmapId}, Student: {StudentId}",
                existingEnrollment.Id, existingEnrollment.RoadmapId, existingEnrollment.StudentId);

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Remove the payment tracking information
            PaymentTrackingStore.RemovePayment(paymentTracking.RoadmapId, paymentTracking.UserId);

            return Result.Success(existingEnrollment.RoadmapId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment redirect: {Message}", ex.Message);
            return Result.Failure<Guid>(Error.Problem("Payment.Error", "An error occurred while processing payment"));
        }
    }
} 