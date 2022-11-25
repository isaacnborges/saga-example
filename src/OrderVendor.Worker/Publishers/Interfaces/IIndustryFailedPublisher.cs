namespace OrderVendor.Worker.Publishers.Interfaces;
public interface IIndustryFailedPublisher
{
    Task Publish(Guid orderId, string customerName);
}
