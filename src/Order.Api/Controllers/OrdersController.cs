using Microsoft.AspNetCore.Mvc;
using Order.Api.Dtos;
using Order.Domain.Interfaces;
using Order.Domain.Services;

namespace Order.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ICartApiService _cartApiService;
    private readonly IPaymentApiService _paymentApiService;
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        ICartApiService cartApiService,
        IPaymentApiService paymentApiService,
        IOrderService orderService,
        ILogger<OrdersController> logger)
    {
        _logger = logger;
        _orderService = orderService;
        _cartApiService = cartApiService;
        _paymentApiService = paymentApiService;
    }

    [HttpPost, Route("finalize", Name = nameof(FinalizeOrders))]
    public async Task<IActionResult> FinalizeOrders()
    {
        _logger.LogInformation("Finalizando pedidos...");

        var cartResponse = await _cartApiService.FinalizeCart();
        _logger.LogInformation($"Carrinho finalizado - {cartResponse}");

        var paymentResponse = await _paymentApiService.PreAuthorizeOrders();
        _logger.LogInformation($"Pedidos pré autorizados - {paymentResponse}");

        var orderResponse = (OrderResponse)await _orderService.CreateOrder();

        return Accepted(orderResponse);
    }
}
