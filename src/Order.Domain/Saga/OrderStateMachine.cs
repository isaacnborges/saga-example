using MassTransit;
using Saga.Contracts;

namespace Order.Domain.Saga;
public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State OrderCreatedState { get; private set; }
    public State PaymentAuthorizedState { get; private set; }
    public State IndustryIntegratedState { get; private set; }
    public State IndustryFailedState { get; private set; }
    public State PaymentConfirmedState { get; private set; }
    public State OrderFinalizeState { get; private set; }

    public Event<OrderCreatedEvent> OrderCreatedEvent { get; private set; }
    public Event<PaymentAuthorizedEvent> PaymentAuthorizedEvent { get; set; }
    public Event<IndustryIntegratedEvent> IndustryIntegratedEvent { get; private set; }
    public Event<IndustryFailedEvent> IndustryFailedEvent { get; private set; }
    public Event<PaymentConfirmedEvent> PaymentConfirmedEvent { get; private set; }
    public Event<OrderProcessedEvent> OrderProcessedEvent { get; private set; }

    public OrderStateMachine()
    {
        ConfigureCorrelationId();

        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderCreatedEvent)
                .Then(context =>
                {
                    context.Saga.CurrentDate = context.Message.Timestamp;
                    context.Saga.CustomerName = context.Message.CustomerName;
                })
                .TransitionTo(OrderCreatedState));

        During(OrderCreatedState,
            When(PaymentAuthorizedEvent)
                .TransitionTo(PaymentAuthorizedState));

        During(PaymentAuthorizedState,
            When(IndustryIntegratedEvent)
                .TransitionTo(IndustryIntegratedState),
            When(IndustryFailedEvent)
                .TransitionTo(IndustryFailedState));

        During(IndustryIntegratedState,
            When(PaymentConfirmedEvent)
                .TransitionTo(PaymentConfirmedState));

        During(PaymentConfirmedState,
            When(OrderProcessedEvent)
                .TransitionTo(OrderFinalizeState));
    }

    private void ConfigureCorrelationId()
    {
        Event(() => OrderCreatedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentAuthorizedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => IndustryIntegratedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => IndustryFailedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentConfirmedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderProcessedEvent, x => x.CorrelateById(m => m.Message.OrderId));
    }
}
