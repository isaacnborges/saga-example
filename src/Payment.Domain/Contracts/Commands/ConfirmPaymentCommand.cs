namespace Saga.Contracts;
public record ConfirmPaymentCommand(Guid OrderId, string CustomerName, DateTime Timestamp);