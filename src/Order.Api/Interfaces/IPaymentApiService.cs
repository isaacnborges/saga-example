namespace Order.Api.Interfaces;

public interface IPaymentApiService
{
    Task<string> PreAuthorizeOrders();
}
