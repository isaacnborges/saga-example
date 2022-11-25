namespace Payment.Worker.Interfaces;
public interface IPaymentConfirmedPublisher
{
    Task Publish(Guid orderId, string customerName);
}
