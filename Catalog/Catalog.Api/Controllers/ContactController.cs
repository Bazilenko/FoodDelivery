using Catalog.Bll.DTOs.Contact;
using Microsoft.AspNetCore.Authorization;
using Catalog.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/owner/contacts")]
    [Authorize(Roles = "RestaurantOwner")]
    [Produces("application/json")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactsController(IContactService service) => _service = service;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactDto>> GetById(int id, CancellationToken ct)
            => Ok(await _service.GetByIdAsync(id, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactDto>> Create(
            [FromBody] ContactCreateDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactDto>> Update(
            int id, [FromBody] ContactUpdateDto dto, CancellationToken ct)
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
