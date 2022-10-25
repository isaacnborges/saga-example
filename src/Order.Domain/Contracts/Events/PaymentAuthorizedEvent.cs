namespace Saga.Contracts;

public record PaymentAuthorizedEvent(Guid OrderId, string CustomerName);