namespace Order.Domain.Interfaces;
public interface IOrderStatusHistoryRepository
{
    Task Add(Models.OrderStatusHistory orderStatusHistory);
}
