namespace Saga.Contracts;

public record OrderCreatedEvent2(Guid OrderId, string CustomerName, DateTime Timestamp);