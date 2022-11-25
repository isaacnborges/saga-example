namespace Order.Worker.Interfaces;
public interface IOrderProcessedPublisher
{
    Task Publish(Guid orderId, string customerName);
}
