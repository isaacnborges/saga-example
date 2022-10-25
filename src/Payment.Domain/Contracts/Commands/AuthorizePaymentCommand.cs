namespace Saga.Contracts;

public record AuthorizePaymentCommand(Guid OrderId, string CustomerName);