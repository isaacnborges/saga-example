namespace Saga.Contracts;

public record AuthorizePaymentCommand2(Guid OrderId, string CustomerName, DateTime Timestamp);
