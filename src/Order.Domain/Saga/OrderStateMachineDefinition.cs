using MassTransit;

namespace Order.Domain.Saga;

public class OrderStateMachineDefinition : SagaDefinition<OrderState>
{
    private readonly IServiceProvider _provider;

    public OrderStateMachineDefinition(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
        //endpointConfigurator.UseMongoDbOutbox(_provider);
    }
}