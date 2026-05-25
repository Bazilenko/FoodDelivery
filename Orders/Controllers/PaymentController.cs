using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Bll.Services.Interfaces;
using Orders.Bll.DTOs.Payment;

namespace Orders.Api.Controllers;
 
[ApiController]
[Route("orders/payments")]
//[Authorize]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
 
    public PaymentsController(IPaymentService service) => _service = service;
 
    // ── POST /api/payments ────────────────────────────────────────────────────
    // Customer submits checkout → creates a Pending payment record.
    // Frontend then calls /process to simulate confirmation.
 
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponseDto>> Create([FromBody] CreatePaymentDto dto)
    {
        var payment = await _service.CreatePaymentAsync(dto.OrderId, dto.PaymentMethod);
        return CreatedAtAction(nameof(Process), new { id = payment.Id }, payment);
    }
 
    // ── POST /api/payments/{id}/process ───────────────────────────────────────
    // Fake payment endpoint — simulates the payment provider webhook.
    // Remove this in production and replace with a real webhook route.
    // Pass ?succeed=false to simulate a card decline.
 
    [HttpPost("{id:int}/process")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Process(
        int id, [FromQuery] bool succeed = true)
    {
        if (succeed)
            await _service.MarkAsPaidAsync(id);
        else
            await _service.MarkAsFailedAsync(id);
 
        return NoContent();
    }
 
    // ── POST /api/payments/{id}/refund ────────────────────────────────────────
 
    [HttpPost("{id:int}/refund")]
    [Authorize(Roles = "Admin,RestaurantOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Refund(int id)
    {
        await _service.RefundAsync(id);
        return NoContent();
    }
}