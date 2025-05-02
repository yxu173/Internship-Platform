using System.Threading.Tasks;
using SharedKernel;

namespace Application.Common.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(
        Guid itemId,
        string name,
        decimal amount,
        string currency,
        Guid userId,
        string customerEmail,
        string customerFirstName,
        string customerLastName,
        string customerPhoneNumber);

    Task<Result> ProcessPaymentCallbackAsync(string payload, string signature);
}

public record PaymentInitiationResponse(
    string CheckoutUrl,
    string TransactionId
);