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

namespace Application.Features.Payments.Commands.ProcessRedirect;

public sealed class ProcessPaymentRedirectCommandHandler : ICommandHandler<ProcessPaymentRedirectCommand>
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

    public async Task<Result> Handle(ProcessPaymentRedirectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment redirect for OrderId: {OrderId}, TransactionId: {TransactionId}",
            request.OrderId, request.TransactionId);

        try
        {
            long orderId = 0;
            if (!string.IsNullOrEmpty(request.OrderId) && !long.TryParse(request.OrderId, out orderId))
            {
                _logger.LogWarning("Invalid OrderId format in payment redirect: {OrderId}", request.OrderId);
            }
            
            var enrollment = await _dbContext.Enrollments
                .Where(e => e.PaymentStatus == PaymentStatus.Pending)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (enrollment == null)
            {
                _logger.LogWarning("Could not find pending enrollment for OrderId: {OrderId}", request.OrderId);
                return Result.Failure(Error.NotFound("Payment.EnrollmentNotFound", "Enrollment not found"));
            }

            
            enrollment.CompletePayment(
                request.TransactionId,
                100.0m // Extract amount from the order or defaults to 100 EGP for now
            );

            _logger.LogInformation("Completing payment for Enrollment {EnrollmentId}, Roadmap: {RoadmapId}, Student: {StudentId}",
                enrollment.Id, enrollment.RoadmapId, enrollment.StudentId);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment redirect: {Message}", ex.Message);
            return Result.Failure(Error.Problem("Payment.Error", "An error occurred while processing payment"));
        }
    }
} 