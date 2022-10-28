namespace Saga.Contracts;
public record IntegrateIndustryCommand(Guid OrderId, string CustomerName, DateTime Timestamp);

