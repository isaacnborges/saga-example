using Order.Api.Interfaces;

namespace Order.Api.ApiServices;

public class PaymentApiService : IPaymentApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentApiService> _logger;

    public PaymentApiService(HttpClient httpClient, ILogger<PaymentApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> PreAuthorizeOrders()
    {
        _logger.LogInformation("Integração com serviço de payment iniciada...");
        var requestUri = "payment/pre-authorize";
        var response = await _httpClient.PostAsync(requestUri, null);
        var responseContent = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Integração com serviço de payment finalizada!");

        return responseContent;
    }
}
