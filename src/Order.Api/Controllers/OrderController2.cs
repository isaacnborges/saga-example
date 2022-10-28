using Microsoft.AspNetCore.Mvc;
using Order.Api.Interfaces;
using Order.Domain.Services;

namespace Order.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController2 : ControllerBase
{
    private readonly ICartApiService _cartApiService;
    private readonly IPaymentApiService _paymentApiService;
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController2> _logger;

    public OrderController2(
        ICartApiService cartApiService,
        IPaymentApiService paymentApiService,
        IOrderService orderService,
        ILogger<OrderController2> logger)
    {
        _logger = logger;
        _orderService = orderService;
        _cartApiService = cartApiService;
        _paymentApiService = paymentApiService;
    }

    [HttpPost, Route("finalize2", Name = nameof(FinalizeOrders2))]
    public async Task<IActionResult> FinalizeOrders2()
    {
        _logger.LogInformation("Finalizando pedidos...");

        var cartResponse = await _cartApiService.FinalizeCart();
        _logger.LogInformation($"Carrinho finalizado - {cartResponse}");

        var paymentResponse = await _paymentApiService.PreAuthorizeOrders();
        _logger.LogInformation($"Pedidos pré autorizados - {paymentResponse}");

        await _orderService.CreateOrder2();

        return Accepted("Pedidos aceitos!");
    }
}
