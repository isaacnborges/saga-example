namespace Order.Worker.Interfaces;
public interface IConfirmPaymentPublisher
{
    Task Publish(Guid orderId, string customerName);
}
