namespace Payment.Worker.Interfaces;
public interface IPaymentAuthorizedPublisher
{
    Task Publish(Guid orderId, string customerName);
}
