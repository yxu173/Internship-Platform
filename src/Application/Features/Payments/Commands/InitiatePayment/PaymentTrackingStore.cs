using System.Collections.Concurrent;
using System.Linq;

namespace Application.Features.Payments.Commands.InitiatePayment;

public static class PaymentTrackingStore
{
    private static readonly ConcurrentDictionary<string, PaymentTrackingInfo> _payments = new();

    public static void StorePayment(PaymentTrackingInfo paymentInfo)
    {
        var key = $"{paymentInfo.RoadmapId}_{paymentInfo.UserId}";
        _payments.TryAdd(key, paymentInfo);
    }

    public static PaymentTrackingInfo? GetPayment(Guid roadmapId, Guid userId)
    {
        var key = $"{roadmapId}_{userId}";
        _payments.TryGetValue(key, out var paymentInfo);
        return paymentInfo;
    }

    public static void RemovePayment(Guid roadmapId, Guid userId)
    {
        var key = $"{roadmapId}_{userId}";
        _payments.TryRemove(key, out _);
    }

    public static PaymentTrackingInfo? GetMostRecentPayment()
    {
        return _payments.Values
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefault();
    }
}

public class PaymentTrackingInfo
{
    public Guid RoadmapId { get; set; }
    public Guid StudentId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
} 