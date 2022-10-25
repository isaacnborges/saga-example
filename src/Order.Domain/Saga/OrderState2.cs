using MassTransit;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.Domain.Saga;
public class OrderState2 : SagaStateMachineInstance, ISagaVersion
{
    [BsonId]
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }

    public string CurrentState { get; set; }
    public DateTime? CurrentDate { get; set; }
    public string? CustomerName { get; set; }
}