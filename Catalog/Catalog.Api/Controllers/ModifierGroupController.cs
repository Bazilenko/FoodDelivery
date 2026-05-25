using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.ModifierGroup;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("catalog/owner/dishes/{dishId:int}/modifier-groups")]
    [Authorize(Roles = "RestaurantOwner")]
    [Produces("application/json")]
    public class ModifierGroupsController : ControllerBase
    {
        private readonly IModifierGroupService _service;

        public ModifierGroupsController(IModifierGroupService service) => _service = service;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModifierGroupDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModifierGroupDto>>> GetByDish(
            int dishId, CancellationToken ct)
            => Ok(await _service.GetByDishAsync(dishId, ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ModifierGroupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModifierGroupDto>> GetById(int id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(id, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ModifierGroupDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ModifierGroupDto>> Create(
            int dishId, [FromBody] ModifierGroupCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dishId, dto, ct);
            return CreatedAtAction(nameof(GetById),
                new { dishId, id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ModifierGroupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModifierGroupDto>> Update(
            int id, [FromBody] ModifierGroupUpdateDto dto, CancellationToken ct)
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