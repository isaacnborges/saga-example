namespace Order.Domain.Interfaces;

public interface IPaymentApiService
{
    Task<string> PreAuthorizeOrders();
}
