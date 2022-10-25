namespace Saga.Contracts;
public record OrderProcessedEvent2(Guid OrderId, string CustomerName, DateTime Timestamp);
