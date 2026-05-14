using Catalog.Bll.DTOs.Address;
using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/owner/addresses")]
    [Authorize(Roles = "RestaurantOwner")]
    [Produces("application/json")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _service;

        public AddressesController(IAddressService service) => _service = service;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddressDto>> GetById(int id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(id, ct));

        [HttpPost]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddressDto>> Create(
            [FromBody] AddressCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddressDto>> Update(
            int id, [FromBody] AddressUpdateDto dto, CancellationToken ct)
        {
            if (id != dto.Id)
                return BadRequest("Route id does not match body id.");

            return Ok(await _service.UpdateAsync(dto, ct));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
