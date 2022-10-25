namespace Saga.Contracts;
public record OrderCreatedEvent(Guid OrderId, string CustomerName);
