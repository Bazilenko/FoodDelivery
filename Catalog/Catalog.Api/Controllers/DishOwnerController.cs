using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.Dish;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("catalog/owner/dishes")]
    [Authorize(Roles = "RestaurantOwner")]
    [Produces("application/json")]
    public class DishesOwnerController : ControllerBase
    {
        private readonly IDishOwnerService _service;

        public DishesOwnerController(IDishOwnerService service) => _service = service;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DishManageDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DishManageDto>>> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DishManageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishManageDto>> GetById(int id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(id, ct));

        [HttpPost]
        [ProducesResponseType(typeof(DishManageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DishManageDto>> Create(
            [FromBody] DishCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DishManageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishManageDto>> Update(
            int id, [FromBody] DishUpdateDto dto, CancellationToken ct)
        {
            if (id != dto.Id)
                return BadRequest("Route id does not match body id.");

            return Ok(await _service.UpdateAsync(dto, ct));
        }

        /// <summary>Toggle a dish's availability without a full update.</summary>
        [HttpPatch("{id:int}/availability")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetAvailability(
            int id, [FromBody] DishAvailabilityDto dto, CancellationToken ct)
        {
            if (id != dto.Id)
                return BadRequest("Route id does not match body id.");

            await _service.SetAvailabilityAsync(dto, ct);
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