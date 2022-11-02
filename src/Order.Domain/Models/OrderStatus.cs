namespace Order.Domain.Models;
public enum OrderStatus
{
    Created,
    PaymentAuthorized,
    IndustryIntegrated,
    PaymentConfirmed,
    Finalize,
    IndustryFailed,
    Canceled
}
