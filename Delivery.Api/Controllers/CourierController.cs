using Delivery.Application.Commands.CourierCommands.Command;
using System.Security.Claims;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Api.Controllers
{
    [ApiController]
    [Route("api/couriers")]
    [Authorize]
    public class CourierController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourierController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CourierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCourierByIdQuery(id), ct);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet("by-phone/{phoneNumber}")]
        [ProducesResponseType(typeof(CourierDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPhoneNumber(string phoneNumber, CancellationToken ct)
        {
            var result = await _mediator.Send(new FindCourierByPhoneNumberQuery(phoneNumber), ct);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCourierCommand cmd, CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCourierRequest request, CancellationToken ct)
        {
            await _mediator.Send(new UpdateCourierCommand(id, request.Name, request.Email, request.PhoneNumber), ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteCourierCommand(id), ct);
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize(Roles = "Courier")]
        [ProducesResponseType(typeof(CourierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var courier = await _mediator.Send(new FindCourierByUserIdQuery(userId), ct);
            if (courier is null)
                return NotFound();

            return Ok(courier);
        }
    }

    public record UpdateCourierRequest(string Name, string Email, string PhoneNumber);
}
