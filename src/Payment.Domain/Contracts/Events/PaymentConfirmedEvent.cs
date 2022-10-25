namespace Saga.Contracts;

public record PaymentConfirmedEvent(Guid OrderId, string CustomerName);
