using MassTransit;

namespace Order.Domain.Saga;

public class OrderStateMachineDefinition2 : SagaDefinition<OrderState2>
{    
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState2> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
        endpointConfigurator.UseInMemoryOutbox();
        //endpointConfigurator.UseMongoDbOutbox();
    }
}