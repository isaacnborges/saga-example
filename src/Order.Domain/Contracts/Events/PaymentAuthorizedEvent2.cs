namespace Saga.Contracts;

public record PaymentAuthorizedEvent2(Guid OrderId, string CustomerName, DateTime Timestamp);