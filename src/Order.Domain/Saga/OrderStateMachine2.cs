using MassTransit;
using Saga.Contracts;

namespace Order.Domain.Saga;
public class OrderStateMachine2 : MassTransitStateMachine<OrderState2>
{
    public State OrderCreatedState { get; private set; }
    public State PaymentAuthorizedState { get; private set; }
    public State IndustryIntegratedState { get; private set; }
    public State PaymentConfirmedState { get; private set; }
    public State OrderFinalizeState { get; private set; }

    public Event<OrderCreatedEvent2> OrderCreatedEvent { get; private set; }
    public Event<PaymentAuthorizedEvent2> PaymentAuthorizedEvent { get; set; }
    public Event<IndustryIntegratedEvent2> IndustryIntegratedEvent { get; private set; }
    public Event<PaymentConfirmedEvent2> PaymentConfirmedEvent { get; private set; }
    public Event<OrderProcessedEvent2> OrderProcessedEvent { get; private set; }

    public OrderStateMachine2()
	{
        Event(() => OrderCreatedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentAuthorizedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => IndustryIntegratedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentConfirmedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderProcessedEvent, x => x.CorrelateById(m => m.Message.OrderId));

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
                .TransitionTo(IndustryIntegratedState));

        During(IndustryIntegratedState,
            When(PaymentConfirmedEvent)
                .TransitionTo(PaymentConfirmedState));

        During(PaymentConfirmedState,
            When(OrderProcessedEvent)
                .TransitionTo(OrderFinalizeState));

        SetCompletedWhenFinalized();
    }
}
