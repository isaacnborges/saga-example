using Microsoft.AspNetCore.Mvc;
using Order.Api.Interfaces;
using Order.Domain.Services;

namespace Order.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ICartApiService _cartApiService;
    private readonly IPaymentApiService _paymentApiService;
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;

    public OrderController(
        ICartApiService cartApiService,
        IPaymentApiService paymentApiService,
        ILogger<OrderController> logger,
        IOrderService orderService)
    {
        _cartApiService = cartApiService;
        _paymentApiService = paymentApiService;
        _logger = logger;
        _orderService = orderService;
    }

    [HttpPost, Route("finalize", Name = nameof(FinalizeOrders))]
    public async Task<IActionResult> FinalizeOrders()
    {
        _logger.LogInformation("Finalizando pedidos...");

        var cartResponse = await _cartApiService.FinalizeCart();
        _logger.LogInformation($"Carrinho finalizado - {cartResponse}");

        var paymentResponse = await _paymentApiService.PreAuthorizeOrders();
        _logger.LogInformation($"Pedidos pré autorizados - {paymentResponse}");

        await _orderService.CreateOrder();

        return Accepted("Pedidos aceitos!");
    }
}
