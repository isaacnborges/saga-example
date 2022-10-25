using Microsoft.AspNetCore.Mvc;

namespace Payment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

    [HttpPost, Route("pre-authorize", Name = nameof(PreAuthorizeOrders))]
    public IActionResult PreAuthorizeOrders()
    {
        _logger.LogInformation("Pagamento pré autorizado");

        return Ok("Pagamento pré autorizado");
    }
}