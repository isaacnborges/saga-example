namespace Order.Domain.Services;
public interface IOrderService
{
    Task<Models.Order> CreateOrder();
}
