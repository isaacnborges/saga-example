namespace Order.Domain.Interfaces;

public interface ICartApiService
{
    Task<string> FinalizeCart();
    Task<string> ReopenCart();
}
