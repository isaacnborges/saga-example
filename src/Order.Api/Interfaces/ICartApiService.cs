namespace Order.Api.Interfaces;

public interface ICartApiService
{
    Task<string> FinalizeCart();
}
