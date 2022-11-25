namespace Order.Worker.Interfaces;
public interface IAuthorizePaymentPublisher
{
    Task Publish(Guid orderId, string customerName);
}
