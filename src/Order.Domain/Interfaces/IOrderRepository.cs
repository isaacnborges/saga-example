namespace Order.Domain.Interfaces;
public interface IOrderRepository
{
    Task Add(Models.Order order);
    Task Update(Models.Order order);
    Task<Models.Order> GetById(Guid id);
}
