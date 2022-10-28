namespace Saga.Contracts;
public record IndustryIntegratedEvent(Guid OrderId, string CustomerName, DateTime Timestamp);
