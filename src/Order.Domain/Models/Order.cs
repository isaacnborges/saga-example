namespace Order.Domain.Models;
public class Order
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public string Cnpj { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; }

    public void UpdateStatus(OrderStatus status) => Status = status;
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal Value { get; set; }
}
