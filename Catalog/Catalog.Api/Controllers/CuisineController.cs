using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Catalog.Bll.DTOs.Cuisine;
using Catalog.Bll.DTOs.Pagination;
using Catalog.Bll.DTOs.Restaurant;


namespace Catalog.Api.Controllers
{
    /// <summary>
    /// Public cuisine list and cuisine-filtered restaurant browse.
    /// Admin write operations require the "Admin" role.
    /// </summary>
    [ApiController]
    [Route("catalog/cuisines")]
    [Produces("application/json")]
    public class CuisinesController : ControllerBase
    {
        private readonly ICuisineService _service;

        public CuisinesController(ICuisineService service) => _service = service;

        /// <summary>All cuisines — used by the main menu filter bar.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CuisineDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CuisineDto>>> GetAll(CancellationToken ct)
        {
            var result = await _service.GetAllAsync(ct);
            return Ok(result);
        }

        /// <summary>Paginated restaurants filtered by cuisine.</summary>
        [HttpGet("{cuisineId:int}/restaurants")]
        [ProducesResponseType(typeof(PagedResult<RestaurantCardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<RestaurantCardDto>>> GetRestaurants(
            int cuisineId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await _service.GetRestaurantsByCuisineAsync(cuisineId, page, pageSize, ct);
            return Ok(result);
        }

        // ── Admin operations ──────────────────────

        [HttpPost]
        [ProducesResponseType(typeof(CuisineDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CuisineDto>> Create(
            [FromBody] CuisineCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetAll), new { }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CuisineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CuisineDto>> Update(
            int id, [FromBody] CuisineUpdateDto dto, CancellationToken ct)
        {
            if (id != dto.Id)
                return BadRequest("Route id does not match body id.");

            var updated = await _service.UpdateAsync(dto, ct);
            return Ok(updated);
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