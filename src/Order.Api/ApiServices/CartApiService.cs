﻿using Order.Api.Interfaces;

namespace Order.Api.ApiServices;

public class CartApiService : ICartApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CartApiService> _logger;

    public CartApiService(HttpClient httpClient, ILogger<CartApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> FinalizeCart()
    {
        _logger.LogInformation("Integração com serviço de cart iniciada...");
        var requestUri = "cart/finalize";
        var response = await _httpClient.PutAsync(requestUri, null);
        var responseContent = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Integração com serviço de cart finalizada!");

        return responseContent;
    }
}