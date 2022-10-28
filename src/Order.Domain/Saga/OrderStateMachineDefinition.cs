using MassTransit;

namespace Order.Domain.Saga;

public class OrderStateMachineDefinition : SagaDefinition<OrderState>
{    
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
        endpointConfigurator.UseInMemoryOutbox();
        //endpointConfigurator.UseMongoDbOutbox();
    }
}