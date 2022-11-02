namespace Order.Domain.Models;
public class Order
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public OrderStatus Status { get; set; }

    public void UpdateStatus(OrderStatus status) => Status = status;
}
