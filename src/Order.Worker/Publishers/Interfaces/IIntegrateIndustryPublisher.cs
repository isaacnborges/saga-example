namespace Order.Worker.Interfaces;
public interface IIntegrateIndustryPublisher
{
    Task Publish(Guid orderId, string customerName);
}
