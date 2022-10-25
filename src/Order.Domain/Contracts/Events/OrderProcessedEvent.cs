namespace Saga.Contracts;
public record OrderProcessedEvent(Guid OrderId, string CustomerName);
