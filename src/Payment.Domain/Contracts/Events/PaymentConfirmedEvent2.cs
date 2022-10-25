namespace Saga.Contracts;
public record PaymentConfirmedEvent2(Guid OrderId, string CustomerName, DateTime Timestamp);
