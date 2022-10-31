namespace Saga.Contracts;
public record IndustryFailedEvent(Guid OrderId, string CustomerName, DateTime Timestamp);