using Microsoft.AspNetCore.Mvc;

namespace Cart.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;

    public CartController(ILogger<CartController> logger)
    {
        _logger = logger;
    }

    [HttpPut, Route("finalize", Name = nameof(FinalizeTruckOrders))]
    public IActionResult FinalizeTruckOrders()
    {
        _logger.LogInformation("Cart finalizado");

        return Ok("Carrinho finalizado");
    }

    [HttpPut, Route("reopen", Name = nameof(ReopenTruckOrders))]
    public IActionResult ReopenTruckOrders()
    {
        _logger.LogInformation("Reabrir cart");

        return Ok("Carrinho reaberto");
    }
}
