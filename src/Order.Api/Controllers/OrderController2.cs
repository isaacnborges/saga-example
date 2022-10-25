using Microsoft.AspNetCore.Mvc;
using Order.Domain.Services;

namespace Order.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController2 : ControllerBase
{
    private readonly ILogger<OrderController2> _logger;
    private readonly IOrderService _orderService;

    public OrderController2(ILogger<OrderController2> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpPost, Route("finalize2", Name = nameof(FinalizeOrders2))]
    public async Task<IActionResult> FinalizeOrders2()
    {
        _logger.LogInformation("Finalizando pedidos...");

        await _orderService.CreateOrder2();

        return Accepted("Pedidos aceitos!");
    }
}
