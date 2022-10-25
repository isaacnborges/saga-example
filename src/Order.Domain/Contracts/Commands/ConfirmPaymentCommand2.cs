namespace Saga.Contracts;
public record ConfirmPaymentCommand2(Guid OrderId, string CustomerName, DateTime Timestamp);