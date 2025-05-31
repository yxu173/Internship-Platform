using MediatR;
using Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Features.Payments.Commands.ProcessCallback;

public class ProcessPaymobCallbackCommandHandler
    : ICommandHandler<ProcessPaymobCallbackCommand>
{
    private readonly IPaymentService _paymentService;

    public ProcessPaymobCallbackCommandHandler(
        IPaymentService paymentService
       )
    {
        _paymentService = paymentService;
    }

    public async Task<Result> Handle(
        ProcessPaymobCallbackCommand request,
        CancellationToken cancellationToken)
    {

        var result = await _paymentService.ProcessPaymentCallbackAsync(
            request.Payload,
            request.Signature
        );
        
        return result;
    }
}