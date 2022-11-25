namespace OrderVendor.Worker.Publishers.Interfaces;
public interface IIndustryIntegratedPublisher
{
    Task Publish(Guid orderId, string customerName);
}
