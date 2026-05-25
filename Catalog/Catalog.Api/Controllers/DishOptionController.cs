using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.DishOption;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("catalog/modifier-groups/owner/{modifierGroupId:int}/options")]
    [Authorize(Roles = "RestaurantOwner")]
    [Produces("application/json")]
    public class DishOptionsController : ControllerBase
    {
        private readonly IDishOptionService _service;

        public DishOptionsController(IDishOptionService service) => _service = service;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DishOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DishOptionDto>>> GetByModifierGroup(
            int modifierGroupId, CancellationToken ct)
            => Ok(await _service.GetByModifierGroupAsync(modifierGroupId, ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DishOptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishOptionDto>> GetById(int id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(id, ct));

        [HttpPost]
        [ProducesResponseType(typeof(DishOptionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DishOptionDto>> Create(
            int modifierGroupId, [FromBody] DishOptionCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(modifierGroupId, dto, ct);
            return CreatedAtAction(nameof(GetById),
                new { modifierGroupId, id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DishOptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishOptionDto>> Update(
            int id, [FromBody] DishOptionUpdateDto dto, CancellationToken ct)
        {
            if (id != dto.Id)
                return BadRequest("Route id does not match body id.");

            return Ok(await _service.UpdateAsync(dto, ct));
        }

        /// <summary>Toggle option availability without a full update.</summary>
        [HttpPatch("{id:int}/availability")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetAvailability(
            int id, [FromBody] bool isAvailable, CancellationToken ct)
        {
            await _service.SetAvailabilityAsync(id, isAvailable, ct);
            return NoContent();
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