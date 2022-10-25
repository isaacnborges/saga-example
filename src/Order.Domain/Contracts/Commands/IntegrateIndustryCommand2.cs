namespace Saga.Contracts;
public record IntegrateIndustryCommand2(Guid OrderId, string CustomerName, DateTime Timestamp);