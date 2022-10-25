using MassTransit;
using Microsoft.Extensions.Logging;
using Saga.Contracts;

namespace Order.Domain.Saga;
public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State PaymentAuthorizedState { get; private set; }
    public State IndustryIntegratedState { get; private set; }
    public State PaymentConfirmedState { get; private set; }

    public Event<OrderCreatedEvent> OrderCreatedEvent { get; private set; }
    public Event<PaymentAuthorizedEvent> PaymentAuthorizedEvent { get; set; }
    public Event<IndustryIntegratedEvent> IndustryIntegratedEvent { get; private set; }
    public Event<PaymentConfirmedEvent> PaymentConfirmedEvent { get; private set; }


    private readonly ILogger<OrderStateMachine> _logger;

    public OrderStateMachine(ILogger<OrderStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.CurrentState);

        //State(() => Processing);

        ConfigureCorrelationIds();

        Initially(SetOrderSummitedHandler());

        During(PaymentAuthorizedState, SetPaymentAuthorizedHandler());
        During(IndustryIntegratedState, SetIndustryIntegratedHandler());
        During(PaymentConfirmedState, SetPaymentConfirmedHandler());

        SetCompletedWhenFinalized();
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => OrderCreatedEvent, x => x.CorrelateById(c => c.Message.OrderId));
        Event(() => PaymentAuthorizedEvent, x => x.CorrelateById(c => c.Message.OrderId));
        Event(() => IndustryIntegratedEvent, x => x.CorrelateById(c => c.Message.OrderId));
        Event(() => PaymentConfirmedEvent, x => x.CorrelateById(c => c.Message.OrderId));
    }

    private EventActivityBinder<OrderState, OrderCreatedEvent> SetOrderSummitedHandler() =>
        When(OrderCreatedEvent)
            .Then(context =>
            {
                context.Saga.CurrentDate = DateTime.Now;
                context.Saga.CustomerName = context.Message.CustomerName;
            })
            .Then(c => _logger.LogInformation($"Order created to {c.Message.OrderId} received"))
            .Publish(c => new AuthorizePaymentCommand(c.Message.OrderId, c.Message.CustomerName))
            .TransitionTo(PaymentAuthorizedState);


    private EventActivityBinder<OrderState, PaymentAuthorizedEvent> SetPaymentAuthorizedHandler() =>
        When(PaymentAuthorizedEvent)
            .Then(context =>
            {
                context.Saga.CurrentDate = DateTime.Now;
                context.Saga.CustomerName = context.Message.CustomerName;
            })
            .Then(c => _logger.LogInformation($"Stock reserved to {c.Message.OrderId} received"))
            .Publish(c => new IntegrateIndustryCommand(c.Message.OrderId, c.Message.CustomerName))
            .TransitionTo(IndustryIntegratedState);


    private EventActivityBinder<OrderState, IndustryIntegratedEvent> SetIndustryIntegratedHandler() =>
        When(IndustryIntegratedEvent)
            .Then(context =>
            {
                context.Saga.CurrentDate = DateTime.Now;
                context.Saga.CustomerName = context.Message.CustomerName;
            })
            .Then(c => _logger.LogInformation($"Payment processed to {c.Message.OrderId} received"))
            .Publish(c => new ConfirmPaymentCommand(c.Message.OrderId, c.Message.CustomerName))
            .TransitionTo(PaymentConfirmedState);

    private EventActivityBinder<OrderState, PaymentConfirmedEvent> SetPaymentConfirmedHandler() =>
        When(PaymentConfirmedEvent)
            .Then(context =>
            {
                context.Saga.CurrentDate = DateTime.Now;
                context.Saga.CustomerName = context.Message.CustomerName;
            })
            .Publish(c => new OrderProcessedEvent(c.Message.OrderId, c.Message.CustomerName))
            //.TransitionTo(IndustryIntegratedState);
            .Finalize();

    //private static void UpdateSagaState(OrderState state)
    //{
    //    state.CurrentDate = DateTime.Now;
    //}
}
