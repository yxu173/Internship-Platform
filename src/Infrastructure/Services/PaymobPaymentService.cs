using Application.Common.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System;
using Application.Abstractions.Data;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Services;

public class PaymobSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string HmacSecret { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://accept.paymob.com/v1";
    public int CardIntegrationId { get; set; }
    public int WalletIntegrationId { get; set; }
}

// DTOs for Paymob Intention API
public class PaymobIntentionRequest
{
    [JsonPropertyName("amount")] public int Amount { get; set; }

    [JsonPropertyName("currency")] public string Currency { get; set; } = "EGP";

    [JsonPropertyName("merchant_order_id")]
    public string? MerchantOrderId { get; set; }

    [JsonPropertyName("payment_methods")] public List<object> PaymentMethods { get; set; } = new();

    [JsonPropertyName("items")] public List<PaymobItem> Items { get; set; } = new();

    [JsonPropertyName("billing_data")] public PaymobBillingData BillingData { get; set; } = new();

    [JsonPropertyName("customer")] public PaymobCustomer Customer { get; set; } = new();

    [JsonPropertyName("notification_url")] public string NotificationUrl { get; set; } = string.Empty;

    [JsonPropertyName("redirection_url")] public string RedirectionUrl { get; set; } = string.Empty;
}

public class PaymobItem
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("amount")] public int Amount { get; set; }
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
    [JsonPropertyName("quantity")] public int Quantity { get; set; } = 1;
}

public class PaymobBillingData
{
    [JsonPropertyName("apartment")] public string Apartment { get; set; } = "NA";
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
    [JsonPropertyName("floor")] public string Floor { get; set; } = "NA";
    [JsonPropertyName("first_name")] public string FirstName { get; set; } = string.Empty;
    [JsonPropertyName("street")] public string Street { get; set; } = "NA";
    [JsonPropertyName("building")] public string Building { get; set; } = "NA";
    [JsonPropertyName("phone_number")] public string PhoneNumber { get; set; } = string.Empty;
    [JsonPropertyName("shipping_method")] public string ShippingMethod { get; set; } = "NA";
    [JsonPropertyName("postal_code")] public string PostalCode { get; set; } = "NA";
    [JsonPropertyName("city")] public string City { get; set; } = "NA";
    [JsonPropertyName("country")] public string Country { get; set; } = "EG";
    [JsonPropertyName("last_name")] public string LastName { get; set; } = string.Empty;
    [JsonPropertyName("state")] public string State { get; set; } = "NA";
}

public class PaymobCustomer
{
    [JsonPropertyName("first_name")] public string FirstName { get; set; } = string.Empty;
    [JsonPropertyName("last_name")] public string LastName { get; set; } = string.Empty;
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
    [JsonPropertyName("phone_number")] public string PhoneNumber { get; set; } = string.Empty;
}

public class PaymobIntentionResponse
{
    [JsonPropertyName("client_secret")] public string ClientSecret { get; set; } = string.Empty;

    [JsonPropertyName("id")] public string IntentionId { get; set; } = string.Empty;
}

public class PaymobTransactionCallbackPayload
{
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;

    [JsonPropertyName("obj")] public PaymobTransactionObject? Obj { get; set; }
}

public class PaymobTransactionObject
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("pending")] public bool Pending { get; set; }

    [JsonPropertyName("amount_cents")] public int AmountCents { get; set; }

    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("is_auth")] public bool IsAuth { get; set; }

    [JsonPropertyName("is_capture")] public bool IsCapture { get; set; }

    [JsonPropertyName("is_standalone_payment")]
    public bool IsStandalonePayment { get; set; }

    [JsonPropertyName("is_voided")] public bool IsVoided { get; set; }

    [JsonPropertyName("is_refunded")] public bool IsRefunded { get; set; }

    [JsonPropertyName("is_3d_secure")] public bool Is3DSecure { get; set; }

    [JsonPropertyName("integration_id")] public int IntegrationId { get; set; }

    [JsonPropertyName("owner")] public int Owner { get; set; }

    [JsonPropertyName("has_parent_transaction")]
    public bool HasParentTransaction { get; set; }

    [JsonPropertyName("order")] public PaymobOrderObject? Order { get; set; }

    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("currency")] public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("source_data")] public PaymobSourceData? SourceData { get; set; }

    [JsonPropertyName("error_occured")] public bool ErrorOccurred { get; set; }

    [JsonPropertyName("is_void")] public bool IsVoid { get; set; }

    [JsonPropertyName("is_refund")] public bool IsRefund { get; set; }

    [JsonIgnore] public string SourceDataPan => SourceData?.Pan ?? "NA";
    [JsonIgnore] public string SourceDataType => SourceData?.Type ?? "NA";
    [JsonIgnore] public string SourceDataSubType => SourceData?.SubType ?? "NA";
    [JsonIgnore] public long OrderId => Order?.Id ?? 0;
}

public class PaymobOrderObject
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }

    [JsonPropertyName("amount_cents")] public int AmountCents { get; set; }

    [JsonPropertyName("currency")] public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("merchant_order_id")]
    public string? MerchantOrderId { get; set; }
}

public class PaymobSourceData
{
    [JsonPropertyName("pan")]
    public string Pan { get; set; } = "NA"; // Last 4 digits usually for card, "N/A" for wallet?

    [JsonPropertyName("type")] public string Type { get; set; } = "NA"; // e.g., "card", "wallet"

    [JsonPropertyName("sub_type")] public string SubType { get; set; } = "NA";
}

public class PaymobPaymentService : IPaymentService
{
    private readonly ILogger<PaymobPaymentService> _logger;
    private readonly PaymobSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationDbContext _context;

    public PaymobPaymentService(
        ILogger<PaymobPaymentService> logger,
        IOptions<PaymobSettings> settings,
        IHttpClientFactory httpClientFactory,
        ApplicationDbContext context)
    {
        _logger = logger;
        _settings = settings.Value;
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    public async Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(
        Guid itemId,
        string name,
        decimal amount,
        string currency,
        Guid userId,
        string customerEmail,
        string customerFirstName,
        string customerLastName,
        string customerPhoneNumber)
    {
        _logger.LogInformation("Initiating Paymob payment intention for item {ItemId} and user {UserId}", itemId,
            userId);

        // Log configuration details
        _logger.LogDebug(
            "Paymob Configuration: BaseUrl={BaseUrl}, ApiKeyLength={ApiKeyLength}, SecretKeyLength={SecretKeyLength}, PublicKeyLength={PublicKeyLength}, CardIntegrationId={CardIntegrationId}, WalletIntegrationId={WalletIntegrationId}",
            _settings.BaseUrl,
            _settings.ApiKey?.Length ?? 0,
            _settings.SecretKey?.Length ?? 0,
            _settings.PublicKey?.Length ?? 0,
            _settings.CardIntegrationId,
            _settings.WalletIntegrationId);

        if (string.IsNullOrWhiteSpace(_settings.SecretKey) || string.IsNullOrWhiteSpace(_settings.PublicKey))
        {
            _logger.LogError("Paymob SecretKey or PublicKey is not configured.");
            return Result.Failure<PaymentInitiationResponse>(Error.BadRequest("Payment.Configuration",
                "Paymob payment service is not configured correctly."));
        }

        int amountInPiastres = (int)(amount * 100); // Convert to smallest currency unit (piastres)

        var requestPayload = new PaymobIntentionRequest
        {
            Amount = amountInPiastres,
            Currency = currency,
            PaymentMethods = new List<object> { 5061257 }, //TODO: Add this into appsettings.json
            MerchantOrderId = $"{itemId}_{userId}",
            NotificationUrl = "https://localhost:7089/api/payments/paymob-callback",
            RedirectionUrl = "https://localhost:7089/api/payments/payment-return",
            Items = new List<PaymobItem>
            {
                new PaymobItem
                {
                    Name = $"Roadmap: {TruncateString(name, 37)}",
                    Amount = amountInPiastres,
                    Description = "Premium Roadmap Access",
                    Quantity = 1
                }
            },
            BillingData = new PaymobBillingData
            {
                Email = customerEmail,
                FirstName = customerFirstName,
                LastName = customerLastName,
                PhoneNumber = customerPhoneNumber,
                Country = "EG",
                City = "NA",
                State = "NA",
                Street = "NA",
                Building = "NA",
                Floor = "NA",
                Apartment = "NA",
                PostalCode = "NA",
                ShippingMethod = "NA"
            },
            Customer = new PaymobCustomer
            {
                Email = customerEmail,
                FirstName = customerFirstName,
                LastName = customerLastName,
                PhoneNumber = customerPhoneNumber
            }
        };

        try
        {
            var httpClient = _httpClientFactory.CreateClient("PaymobClient");
            httpClient.BaseAddress =
                new Uri(_settings.BaseUrl.EndsWith("/") ? _settings.BaseUrl : _settings.BaseUrl + "/");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_settings.SecretKey}");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var jsonPayload =
                JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions { WriteIndented = true });

            HttpResponseMessage response = await httpClient.PostAsJsonAsync("intention/", requestPayload);

            if (response.IsSuccessStatusCode)
            {
                var intentionResponse = await response.Content.ReadFromJsonAsync<PaymobIntentionResponse>();
                if (intentionResponse != null && !string.IsNullOrWhiteSpace(intentionResponse.ClientSecret))
                {
                    _logger.LogInformation(
                        "Successfully created Paymob intention. IntentionId: {IntentionId}, MerchantOrderId: {MerchantOrderId}",
                        intentionResponse.IntentionId, requestPayload.MerchantOrderId);

                    var checkoutUrl =
                        $"https://accept.paymob.com/unifiedcheckout/?publicKey={_settings.PublicKey}&clientSecret={intentionResponse.ClientSecret}";

                    var result = new PaymentInitiationResponse(checkoutUrl, intentionResponse.IntentionId);
                    return Result.Success(result);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Result.Failure<PaymentInitiationResponse>(Error.Problem("Payment.InvalidResponse",
                        $"Failed to create Paymob intention: Invalid response. Details: {errorContent}"));
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure<PaymentInitiationResponse>(Error.BadRequest("Payment.Failed",
                    $"Failed to create Paymob intention: {response.ReasonPhrase}. Details: {errorContent}"));
            }
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<PaymentInitiationResponse>(Error.Problem("Payment.Network",
                "Network error occurred while contacting payment provider."));
        }
        catch (Exception ex)
        {
            return Result.Failure<PaymentInitiationResponse>(Error.Problem("Payment.Unexpected",
                "An unexpected error occurred."));
        }
    }

    public async Task<Result> ProcessPaymentCallbackAsync(string payload, string receivedHmac)
    {
        if (string.IsNullOrWhiteSpace(_settings.HmacSecret))
        {
            return Result.Failure(Error.BadRequest("Payment.Configuration", "Payment callback configuration error."));
        }

        if (string.IsNullOrWhiteSpace(receivedHmac))
        {
            return Result.Failure(Error.BadRequest("Payment.Signature", "Missing HMAC signature."));
        }

        PaymobTransactionCallbackPayload? callbackData;
        try
        {
            callbackData = JsonSerializer.Deserialize<PaymobTransactionCallbackPayload>(payload,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (callbackData == null || callbackData.Obj == null ||
                callbackData.Type?.ToUpperInvariant() != "TRANSACTION")
            {
                return Result.Failure(Error.BadRequest("Payment.Payload", "Invalid callback payload format."));
            }
        }
        catch (JsonException ex)
        {
            return Result.Failure(Error.BadRequest("Payment.Format", "Failed to parse callback payload."));
        }

        if (!VerifyHmac(callbackData.Obj, receivedHmac, _settings.HmacSecret))
        {
            return Result.Failure(Error.BadRequest("Payment.Verification", "Invalid HMAC signature."));
        }


        if (!callbackData.Obj.Success || callbackData.Obj.Pending)
        {
            return Result.Success();
        }


        if (string.IsNullOrWhiteSpace(callbackData.Obj.Order?.MerchantOrderId))
        {
            return Result.Failure(Error.BadRequest("Payment.MerchantId",
                "Cannot process successful transaction: Missing Merchant Order ID."));
        }

        string[] idParts = callbackData.Obj.Order.MerchantOrderId.Split('_');
        if (idParts.Length < 2 || !Guid.TryParse(idParts[0], out Guid roadmapId) ||
            !Guid.TryParse(idParts[1], out Guid studentId))
        {
            return Result.Failure(Error.BadRequest("Payment.MerchantIdFormat",
                "Cannot process successful transaction: Invalid Merchant Order ID format."));
        }

        try
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.RoadmapId == roadmapId && e.StudentId == studentId);

            if (enrollment == null)
            {
                return Result.Failure(Error.NotFound("Payment.Enrollment",
                    "Cannot process successful transaction: Enrollment record not found."));
            }

            if (enrollment.PaymentStatus == PaymentStatus.Completed)
            {
                return Result.Success();
            }

            // TODO: Add amount validation if needed
            // decimal expectedAmount = ... fetch roadmap price again ... * 100;
            // if (callbackData.Obj.AmountCents != (int)expectedAmount) { ... log warning/error ... }

            enrollment.CompletePayment(
                callbackData.Obj.Id.ToString(),
                callbackData.Obj.AmountCents / 100.0m
            );

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Successfully completed enrollment for Student {StudentId} in Roadmap {RoadmapId}. Transaction ID: {TransactionId}",
                studentId, roadmapId, callbackData.Obj.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("Payment.Database",
                "Failed to update application state after successful payment."));
        }
    }

    private bool VerifyHmac(PaymobTransactionObject obj, string receivedHmac, string hmacSecret)
    {
        var concatenatedData = new StringBuilder();
        concatenatedData.Append(obj.AmountCents);
        concatenatedData.Append(obj.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ"));
        concatenatedData.Append(obj.Currency);
        concatenatedData.Append(obj.ErrorOccurred.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.HasParentTransaction.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.Id);
        concatenatedData.Append(obj.IntegrationId);
        concatenatedData.Append(obj.Is3DSecure.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.IsAuth.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.IsCapture.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.IsRefunded.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.IsStandalonePayment.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.IsVoided.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.OrderId);
        concatenatedData.Append(obj.Owner);
        concatenatedData.Append(obj.Pending.ToString().ToLowerInvariant());
        concatenatedData.Append(obj.SourceDataPan);
        concatenatedData.Append(obj.SourceDataSubType);
        concatenatedData.Append(obj.SourceDataType);
        concatenatedData.Append(obj.Success.ToString().ToLowerInvariant());

        string dataString = concatenatedData.ToString();

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hmacSecret));
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataString));

        string calculatedHmac = Convert.ToHexString(hashBytes).ToLowerInvariant();


        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(calculatedHmac),
            Encoding.UTF8.GetBytes(receivedHmac)
        );
    }

    private string TruncateString(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        const string ellipsis = "...";
        return input.Length <= maxLength
            ? input
            : input.Substring(0, maxLength - ellipsis.Length) + ellipsis;
    }
}