using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Api.Controllers
{
    [ApiController]
    [Route("api/deliveries")]
    [Authorize]
    public class DeliveryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateDeliveryCommand cmd, CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(GetByOrderId), new { orderId = cmd.OrderId }, id);
        }

        [HttpPut("{id}/courier")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(DeliveryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignCourier(string id, [FromBody] AssignCourierRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new AssignCourierToDeliveryCommand(id, request.CourierId), ct);
            return Ok(result);
        }

        [HttpGet("ready-for-pickup")]
        [Authorize(Roles = "Courier")]
        [ProducesResponseType(typeof(IEnumerable<DeliveryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReadyForPickup(CancellationToken ct)
        {
            return Ok(await _mediator.Send(new GetReadyForPickupQuery(), ct));
        }

        [HttpGet("courier/{courierId}/active")]
        [Authorize(Roles = "Courier")]
        [ProducesResponseType(typeof(DeliveryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveByCourier(string courierId, CancellationToken ct)
        {
            return Ok(await _mediator.Send(new GetActiveByCourierQuery(courierId), ct));
        }

        [HttpGet("courier/{courierId}/history")]
        [Authorize(Roles = "Courier")]
        [ProducesResponseType(typeof(IEnumerable<DeliveryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoryByCourier(string courierId, CancellationToken ct)
        {
            return Ok(await _mediator.Send(new GetDeliveryHistoryByCourierQuery(courierId), ct));
        }

        [HttpPost("{id}/pick-up")]
        [Authorize(Roles = "Courier")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkInTransit(string id, CancellationToken ct)
        {
            await _mediator.Send(new MarkInTransitCommand(id), ct);
            return NoContent();
        }

        [HttpPost("{id}/delivered")]
        [Authorize(Roles = "Courier")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkDelivered(string id, CancellationToken ct)
        {
            await _mediator.Send(new MarkDeliveredCommand(id), ct);
            return NoContent();
        }

        [HttpPost("{id}/failed")]
        [Authorize(Roles = "Courier,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkFailed(string id, [FromBody] MarkFailedRequest request, CancellationToken ct)
        {
            await _mediator.Send(new MarkFailedCommand(id, request.Reason), ct);
            return NoContent();
        }

        [HttpGet("order/{orderId}")]
        [ProducesResponseType(typeof(DeliveryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByOrderId(int orderId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetDeliveryByOrderIdQuery(orderId), ct);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<DeliveryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            return Ok(await _mediator.Send(new GetAllDeliveriesQuery(), ct));
        }
    }

    public record AssignCourierRequest(string CourierId);
    public record MarkFailedRequest(string Reason);
}