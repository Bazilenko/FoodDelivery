using Catalog.Bll.DTOs.Category;
using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/owner/categories")]
    [Authorize(Roles = "RestaurantOwner")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service) => _service = service;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(id, ct));

        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> Create(
            [FromBody] CategoryCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> Update(
            int id, [FromBody] CategoryUpdateDto dto, CancellationToken ct)
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
