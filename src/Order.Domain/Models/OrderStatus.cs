using System.ComponentModel;

namespace Order.Domain.Models;
public enum OrderStatus
{
    [Description("Order Created")]
    Created,

    [Description("Payment Authorized")]
    PaymentAuthorized,

    [Description("Order Integrated with Industry")]
    IndustryIntegrated,

    [Description("Payment Confirmed")]
    PaymentConfirmed,

    [Description("Order Finalized")]
    Finalized,

    [Description("Order Industry Failed")]
    IndustryFailed,

    [Description("Order Canceled")]
    Canceled
}
