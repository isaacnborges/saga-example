namespace Order.Domain.Models;
public class OrderStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime UpdatedDate { get; set; }

    public OrderStatusHistory(Guid orderId, OrderStatus orderStatus)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        UpdatedDate = DateTime.Now;
    }
}
